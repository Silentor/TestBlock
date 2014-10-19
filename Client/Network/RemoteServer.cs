using System;
using System.Diagnostics;
using System.IO;
using Lidgren.Network;
using NLog;
using Silentor.TB.Client.Config;
using Silentor.TB.Client.Tools;
using Silentor.TB.Client.Tools;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Common.Network.Serialization;
using Silentor.TB.Common.Tools;
using UnityEngine;

namespace Silentor.TB.Client.Network
{
    public class RemoteServer  : IServer, IClientServer, IServerClient
    {
        public bool IsConnected { get { return _connection.ConnectionStatus == NetConnectionStatus.Connected; } }

        public int RecvBytes { get { return _connection.ServerConnection.Statistics.ReceivedBytes; } }

        public int SendBytes { get { return _connection.ServerConnection.Statistics.SentBytes; } }

        public float RTT { get { return _connection.ServerConnection.AverageRoundtripTime; } }

        public IClientServer ServerConnection { get { return this; } }

        public IServerClient ClientConnection { get { return this; } }

        //private AverageTimer _receiveMessageTimer = new AverageTimer();

        public RemoteServer(ISystemConfig config, IApplicationEvents applicationEvents)
        {
            var netConfig = new NetPeerConfiguration("Silentor.WoB");
            //netConfig.ReceiveBufferSize = 100*1024*1024;
            _connection = new NetClient(netConfig);
            _connection.Start();
            var hail = _connection.CreateMessage("Connect to WoB server");
			_connection.Connect(config.ServerAddress, config.ServerPort, hail);

            Log.Trace("Connecting to {0} : {1} ", config.ServerAddress, config.ServerPort);

            _applicationEvents = applicationEvents;
            _applicationEvents.FrameTick += ApplicationEventsFrameTick;
            _applicationEvents.Closed += ApplicationEventsOnClosed;

            _serializer = new MessageSerializer(() => _connection.CreateMessage());

            Log.Info("Server started");
        }

        public void GetChunk(Vector2i position)
        {
            var chunkRequestMessage = new ChunkRequestMessage(position);
            SendMessage(chunkRequestMessage);

            Log.Debug("...-> Chunk {0} requested", position);
        }

        public void MovePlayer(Vector3 movement, Vector2 rotation, bool jump = false)
        {
            var playerMovement = new PlayerMovement (movement.ToProtoVector(), rotation.ToProtoVector(), jump);
            SendMessage(playerMovement);

            Log.Debug("...-> Player movement {0}, rotation {1}, jump {2} send", movement, rotation, jump);
        }

        public void AddPlayer(string name)
        {
            if(String.IsNullOrEmpty(name)) throw new ArgumentException("Name is not defined", "name");

            var loginData = new LoginData(name);
            SendMessage(loginData);

            Log.Info("...-> Login player \"{0}\"", name);
        }

        public void Disconnect()
        {
            _connection.Disconnect("Client has disconnected");
        }

        public event Action Connected;
        public event Action<LoginResponce> Logined;
        public event Action<ChunkContents> ChunkReceived;
        public event Action<EntityUpdate> PositionUpdated;

        private readonly IApplicationEvents _applicationEvents;

        private readonly NetClient _connection;
        private Vector3i _playerPos;
        private int _sightRadius;
        private readonly MessageSerializer _serializer;

        private static readonly Logger Log = LogManager.GetLogger("Client.Network.RemoteServer");

        //Stream receive stuff
        private readonly MemoryStream _receivingStream = new MemoryStream();
        private int _streamLength;
        private int _streamReceiverTimer;

        private void ApplicationEventsFrameTick()
        {
            GotMessage(null);
        }

        private void SendMessage(Message msg)
        {
            var buffer = (NetOutgoingMessage)_serializer.Serialize(msg);
            _connection.SendMessage(buffer, msg.Delivery.Method, msg.Delivery.Channel);
        }

        private void DoChunkReceived(ChunkContents chunk)
        {
            Log.Info("...<- Received chunk {0}", chunk.Position);

            if (ChunkReceived != null)
                ChunkReceived(chunk);
        }

        private void DoLogined(LoginResponce loginResponce)
        {
            Log.Info("...<- Received login responce for {0}, start position: {1}", loginResponce.Id, loginResponce.Position);

            if (Logined != null)
                Logined(loginResponce);
        }

        private void DoPositionUpdated(EntityUpdate position)
        {
            Log.Info("...<- Received position update: {0}", position);

            if (PositionUpdated != null)
                PositionUpdated(position);
        }

        private void DoConnected()
        {
            if (Connected != null)
                Connected();
        }

        private void GotMessage(object peer)
        {
            var timer = Stopwatch.StartNew();
            var messagesCount = 0;

            NetIncomingMessage im;
            while ((im = _connection.ReadMessage()) != null)
            {
                Log.Trace("Received message: {0} - {1} - {2} bytes", im.MessageType, im.DeliveryMethod, im.LengthBytes);

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
                        var status = (NetConnectionStatus)im.ReadByte();
                        var reason = im.ReadString();
                        if (status == NetConnectionStatus.Connected)
                        {
                            Log.Info("Connected to server {0}, reason: {1}", im.SenderEndPoint.ToString(), reason);

                            DoConnected();
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                            Log.Info("Disconnected from {0}, reason: {1}", im.SenderEndPoint.ToString(), reason);
                        else
                        {
                            Log.Info("{0} from {1}, reason: {2}", status, im.SenderEndPoint.ToString(), reason);
                        }
                        break;

                    case NetIncomingMessageType.Data:
                        messagesCount++;
                        //if (im.SequenceChannel == Settings.ChunkChannel &&
                        //    im.DeliveryMethod == NetDeliveryMethod.ReliableOrdered)
                        //    ProcessStreamMessage(im);
                        //else
                        var isCompressed = Settings.Chunk.IsSameAs(im);
                        ProcessDataMessage(im, isCompressed);
                        break;

                    default:
                        Log.Warn("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                _connection.Recycle(im);

                if (timer.ElapsedMilliseconds > 15)
                    break;
            }

            timer.Stop();

            if(messagesCount > 0 && Log.IsTraceEnabled)
                Log.Trace("Received and processed {0} user messages, time {1} ms", messagesCount, timer.ElapsedMilliseconds);
        }

        //private void ProcessStreamMessage(NetIncomingMessage im)
        //{
        //    //Header packet (stream length and some start data)
        //    if (_receivingStream.Length == 0)
        //    {
        //        var data = im.ReadBytes(im.LengthBytes);
        //        var command = (StreamHeader)_serializer.Decode(data);

        //        _streamReceiverTimer = Environment.TickCount;
        //        _streamLength = command.ContentLength;

        //        //Stream is small and received in header packet
        //        if (command.ContentLength == command.ContentStart.Length)
        //        {
        //            Log.Trace("Received short stream, size {0}, time {1} msec", command.ContentLength,
        //                Environment.TickCount - _streamReceiverTimer);

        //            var chunkContent = (ChunkContents) _serializer.Decode(command.ContentStart, true);
        //            DoChunkReceived(chunkContent);
        //        }
        //        else
        //            _receivingStream.Write(command.ContentStart, 0, command.ContentStart.Length);
        //    }
        //        //Stream content packet
        //    else
        //    {
        //        var data = im.ReadBytes(im.LengthBytes);
        //        _receivingStream.Write(data, 0, data.Length);

        //        //Stream is received already
        //        if (_receivingStream.Position == _streamLength)
        //        {
        //            Log.Trace("Received long stream, size {0}, time {1} msec", _receivingStream.Length, Environment.TickCount - _streamReceiverTimer);

        //            _receivingStream.Position = 0;
        //            var chunkContent = (ChunkContents)_serializer.Decode(_receivingStream.ToArray(), true);
        //            DoChunkReceived(chunkContent);
        //            _receivingStream.SetLength(0);
        //        }
        //    }
        //}
        
        private void ProcessDataMessage(NetIncomingMessage im, bool isCompressed = false)
        {
            var message = _serializer.Deserialize(im);

            switch (message.Header)
            {
                case Headers.LoginResponce:
                    var loginData = (LoginResponce)message;
                    DoLogined(loginData);
                    break;

                case Headers.ChunkResponce:
                    var chunkData = (ChunkMessage)message;
                    var chunkContent = new ChunkContents(chunkData.Position, chunkData.Blocks, chunkData.HeightMap);
                    DoChunkReceived(chunkContent);
                    break;

                case Headers.EntityUpdate:
                    var positionData = (EntityUpdate)message;
                    DoPositionUpdated(positionData);
                    break;

                default:
                    Log.Warn("Unexpected message header: {0}", (byte)message.Header);
                    break;
            }
        }

        private void ApplicationEventsOnClosed()
        {
            Disconnect();
        }
    }
}
