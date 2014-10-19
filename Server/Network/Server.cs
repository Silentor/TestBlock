﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Lidgren.Network;
using NLog;

using Silentor.TB.Common.Network;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Common.Network.Serialization;
using Silentor.TB.Server.Players;
using Silentor.TB.Server.Tools;

namespace Silentor.TB.Server.Network
{
    /// <summary>
    /// Lidgren Network based server. Containts pull-based buffer of incoming messages
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Fires on message received for logined client
        /// </summary>
        public IObservable<IncomingEnvelop> MessageReceived
        {
            get { return _decodeToConsumer.AsObservable(); }
        }

        public Server(int port = 10000, int securityPort = 9999)
        {
            var config = new NetPeerConfiguration(Settings.AppIdentifier);
            config.MaximumConnections = 100;
            config.Port = port;
            _server = new NetServer(config);
            _server.Start();

            SetupSecurity(((IPEndPoint)_server.Socket.LocalEndPoint).Address, securityPort);

            Log.Info("Server starting at port {0}", _server.Port);

            //Create receiving workers
            _networkWorker = new Thread(ReadMessages)
            {
                Name = "Wob.Server.Network.Server.ReadMessages",
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            _networkWorker.Start();
            _decode = new TransformBlock<NetIncomingMessage, IncomingEnvelop>(msg => DecodeMessages(msg), 
                new ExecutionDataflowBlockOptions(){SingleProducerConstrained = true});

            //Create sending workers
            _producerToEncode = new BufferBlock<OutgoingEnvelop>();
            _encode = new TransformBlock<OutgoingEnvelop, OutgoingEnvelop>(msg => EncodeMessages(msg));
            _send = new ActionBlock<OutgoingEnvelop>(msg => WriteMessages(msg));

            //Create links
            var propagateOption = new DataflowLinkOptions() {PropagateCompletion = true};
            _readToDecode.LinkTo(_decode, propagateOption);
            _decode.LinkTo(_decodeToConsumer, propagateOption, inc => inc.Message != null);
            _decode.LinkTo(new ActionBlock<IncomingEnvelop>(ie =>
            {
                Log.Trace("Nonprocessible message discarded from {0}", ie.Connection.RemoteEndPoint);
                _errorPacketsCount++;
            }));

            _producerToEncode.LinkTo(_encode, propagateOption);
            _encode.LinkTo(_send, propagateOption);
        }

        /// <summary>
        /// Enqueue message to send
        /// </summary>
        /// <param name="message"></param>
        /// <param name="client"></param>
        public void Send(Message message, Session client)
        {
            _producerToEncode.Post(new OutgoingEnvelop { Client = client, Message = message });
        }

        public void CompleteSend()
        {
            _readToDecode.Complete();
        }

        public void Stop()
        {
            _isStopping = true;
        }

        /// <summary>
        /// Get approx statistic snapshot
        /// </summary>
        /// <returns></returns>
        public Statistic GetStatistic()
        {
            return new Statistic(this);
        }

        #region Dataflow Pipeline

        private readonly BufferBlock<NetIncomingMessage> _readToDecode = new BufferBlock<NetIncomingMessage>();
        private readonly TransformBlock<NetIncomingMessage, IncomingEnvelop> _decode;
        private readonly BufferBlock<IncomingEnvelop> _decodeToConsumer = new BufferBlock<IncomingEnvelop>();

        private readonly BufferBlock<OutgoingEnvelop> _producerToEncode = new BufferBlock<OutgoingEnvelop>();
        private readonly TransformBlock<OutgoingEnvelop, OutgoingEnvelop> _encode;
        private readonly ActionBlock<OutgoingEnvelop> _send;

        /// <summary>
        /// Read raw messages and direct its for decode and process
        /// </summary>
        private void ReadMessages()
        {
            Log.Trace("Start ReadMessages stage");

            try
            {
                while (true)
                {
                    NetIncomingMessage im;
                    if ((im = _server.WaitMessage(100)) != null)
                    {
                        // handle incoming message
                        switch (im.MessageType)
                        {
                            case NetIncomingMessageType.VerboseDebugMessage:
                                Log.Trace("Lidgren: {0}", im.ReadString());
                                break;
                            case NetIncomingMessageType.DebugMessage:
                                Log.Debug("Lidgren: {0}", im.ReadString());
                                break;
                            case NetIncomingMessageType.ErrorMessage:
                                Log.Error("Lidgren: {0}", im.ReadString());
                                break;
                            case NetIncomingMessageType.WarningMessage:
                                Log.Warn("Lidgren: {0}", im.ReadString());
                                break;

                            case NetIncomingMessageType.StatusChanged:
                            case NetIncomingMessageType.Data:
                                //todo check message for IP ban list
                                if (Log.IsTraceEnabled)
                                    Log.Trace("Received message {0}, send to decode", im.MessageType);

                                _readToDecode.Post(im);
                                break;

                            default:
                                Log.Warn("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " +
                                         im.DeliveryMethod + "|" + im.SequenceChannel);
                                break;
                        }
                    }

                    if (_isStopping)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in ReadMessages()", ex);
                throw;
            }
            finally
            {
                //Incoming message queue is completed - server stopped
                _readToDecode.Complete();

                Log.Trace("Exit from ReadMessages stage");
            }
        }
        

        /// <summary>
        /// Decode and dispatch received messages. Dispatched by block links. Threadsafe.
        /// </summary>
        /// <param name="im"></param>
        /// <returns></returns>
        private IncomingEnvelop DecodeMessages(NetIncomingMessage im)
        {
            Log.Trace("Decoding message {0}", im);

            try
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:

                        var status = (NetConnectionStatus) im.ReadByte();
                        if (status == NetConnectionStatus.Connected)
                        {
                            //Ok, just waiting for proper login
                            Log.Info("Connected from {0}, waiting for login", im.SenderConnection);
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {
                            //Notify if player was connected
                            if (im.SenderConnection.Tag != null)
                            {
                                var disconnect = new IncomingEnvelop
                                {
                                    Client = (Simulator) im.SenderConnection.Tag,
                                    Message = new Disconnect(),
                                    RecvBuffer = im
                                };
                                Log.Info("Disconnected from " + im.SenderEndPoint);

                                return disconnect;
                            }
                        }
                        else
                        {
                            var reason = im.ReadString();
                            Log.Info("Status changed for " + im.SenderConnection + ": " + status +
                                     ": " + reason);
                        }
                        break;

                    case NetIncomingMessageType.Data:
                    {
                        //Deserialize message
                        var command = _serializer.Deserialize(im);

                        //Drop messages from unlogined clients except login messages
                        if (im.SenderConnection.Tag == null && command.Header != Headers.Login)
                        {
                            Log.Warn("Message {0} from unlogined client {1}", command.Header, im.SenderConnection.RemoteEndPoint);
                            break;
                        }

                        //Envelope message
                        var incoming = new IncomingEnvelop
                        {
                            RecvBuffer = im,
                            Message = command,
                            Client = (Simulator) im.SenderConnection.Tag,
                        };

                        //Route message
                        return incoming;
                    }

                    default:
                        Log.Warn("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " +
                                 im.DeliveryMethod + "|" + im.SequenceChannel);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in DecodeMessages()", ex);
                throw;
            }

            return new IncomingEnvelop(){RecvBuffer = im };
        }

        private OutgoingEnvelop EncodeMessages(OutgoingEnvelop envelope)
        {
            try
            {
                //var serializer = GetSerializer();
                var sendBuffer = envelope.Client.CreateSendBuffer();
                envelope.Message.Serialize(sendBuffer);
                envelope.SendBuffer = sendBuffer;

                return envelope;
            }
            catch (Exception ex)
            {
                Log.Error("Exception in EncodeMessages()", ex);
                throw;
            }
        }

        /// <summary>
        /// Parallelize by client connection
        /// </summary>
        /// <param name="envelope"></param>
        private async Task WriteMessages(OutgoingEnvelop envelope)
        {
            try
            {
                while (true)
                {
                    int windowSize, freeWindowSlots;
                    envelope.Client.Connection.GetSendQueueInfo(envelope.Message.Delivery.Method,
                        envelope.Message.Delivery.Channel,
                        out windowSize, out freeWindowSlots);

                    if (freeWindowSlots > 0)
                    {
                        var result = envelope.Client.Connection.SendMessage(envelope.SendBuffer,
                            envelope.Message.Delivery.Method,
                            envelope.Message.Delivery.Channel);
                        if (result == NetSendResult.Dropped || result == NetSendResult.FailedNotConnected)
                            Log.Error("Message was not send, result: " + result);
                        break;
                    }
                    else
                        await Task.Delay(1);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception in WriteMessages()", ex);
                throw;
            }
        } 
        #endregion

        private void SetupSecurity(IPAddress local, int port)
        {
            _securitySocket = new TcpListener(local, port);
            _securitySocket.Start();
            Task.Run(() => ProcessSecurity());
        }

        private async void ProcessSecurity()
        {
            while (!_isStopping)
            {
                var newClient = await _securitySocket.AcceptTcpClientAsync();
                var result = await newClient.Client.SendTaskAsync(System.Text.Encoding.ASCII.GetBytes(SocketPolicy));

                Log.Debug("Socket security sended to {0}", newClient.Client.RemoteEndPoint);
                newClient.Close();
            }
        }

        private const string SocketPolicy = @"
<?xml version=""1.0""?>
<cross-domain-policy>
   <allow-access-from domain=""*"" to-ports=""9998-10001""/> 
</cross-domain-policy>";
        

        private readonly NetServer _server;
        private readonly Thread _networkWorker;

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly List<Session> _clients = new List<Session>();
        private readonly MessageSerializer _serializer = new MessageSerializer();

        //Pool of message encode/decode serializers
        //private readonly ConcurrentBag<CommandSerializer> _serializers = new ConcurrentBag<CommandSerializer>();

        private bool _isStopping;
        private int _errorPacketsCount;
        private TcpListener _securitySocket;

        public struct Statistic
        {
            public readonly int IncomingCount;
            public readonly int DecodedCount;
            public readonly int ErrorCount;
            public readonly int EncodingCount;
            public readonly int OutgoingCount;

            public Statistic(Server server)
            {
                OutgoingCount = server._encode.OutputCount;
                EncodingCount = server._encode.InputCount;
                DecodedCount = server._decodeToConsumer.Count;
                IncomingCount = server._decode.InputCount;
                ErrorCount = server._errorPacketsCount;
            }
        }
    }
}
