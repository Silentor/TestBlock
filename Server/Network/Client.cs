using System;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using NLog;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Common.Network.Serialization;
using Silentor.TB.Common.Tools;
using Silentor.TB.Server.Players;
using Silentor.TB.Server.Tools;

namespace Silentor.TB.Server.Network
{
    /// <summary>
    /// Network connection from server to client
    /// </summary>
    public class Client : IMessageReceiver
    {
        public NetConnection Connection { get { return _connection; } }

        /// <summary>
        /// Client requests, not so urgent
        /// </summary>
        public ISourceBlock<ClientRequest> ClientRequestReceived
        {
            get { return _clientRequests; }
        }

        /// <summary>
        /// Hero action, needs urgent processing
        /// </summary>
        public Action<HeroAction> HeroActionReceived { get; set; }

        public Client([NotNull] NetConnection clientConnection, [NotNull] Server server,
            [NotNull] MessageFactory messageFactory, int id)
        {
            if (clientConnection == null) throw new ArgumentNullException("clientConnection");
            if (server == null) throw new ArgumentNullException("server");
            if (messageFactory == null) throw new ArgumentNullException("messageFactory");

            _connection = clientConnection;
            _server = server;
            _id = id;
            _client = _connection.Peer;
            _serializer = new MessageSerializer(messageFactory);

            Log = LogManager.GetLogger(GetType().FullName + id);
            Log.Debug("Created client");
        }

        public NetOutgoingMessage Serialize(Message message)
        {
            var buffer = _client.CreateMessage(message.Size + MessageSerializer.HeaderSize);
            _serializer.Serialize(message, buffer);

            Log.Debug("Serialized message {0} to buffer size {1}", message.Header, buffer.LengthBytes);

            return buffer;
        }

        public void LoginAccept(int id, Vector3 startPosition, Quaternion rotation, int simulationSize, HeroController hero)
        {
            var message = new LoginResponce(id, startPosition.ToProtoVector(), rotation.ToProtoQuaternion(), 
                simulationSize);
            _connection.Tag = hero;
            _server.Send(message, this);

            Log.Info("...<- Sending login acceptation for {0}", id);
        }

        public void SendPosition(int id, Vector3 position, Quaternion rotation, bool isRemoved = false)
        {
            var playerPosition = new EntityUpdate(id, position.ToProtoVector(), rotation.ToProtoQuaternion(), isRemoved);
            _server.Send(playerPosition, this);

            Log.Info("...<- Sending new position {0}", playerPosition);
        }

        /// <summary>
        /// Send chunk content
        /// </summary>
        /// <param name="chunkData"></param>
        public void SendChunk(ChunkContents chunkData)
        {
            _server.Send(new ChunkMessage(chunkData), this);
            Log.Info("...<- Sending chunk {0}", chunkData.Position);
        }

        /// <summary>
        /// Process time consuming streaming operations. Small packets sends immediately
        /// </summary>
        //public void StreamSendUpdate()
        //{
        //    //Retreive stream for sending, if any
        //    if(_currentStream == null)
        //        if (!_streamsToSend.IsEmpty)
        //        {
        //            _streamsToSend.TryDequeue(out _currentStream);
        //            _currentStreamPosition = 0;
        //        }
        //        else
        //            return;

        //    int windowSize, freeWindowSlots;
        //    _connection.GetSendQueueInfo(NetDeliveryMethod.ReliableOrdered, Settings.StreamChannel, out windowSize, out freeWindowSlots);
        //    if (freeWindowSlots > 0)
        //    {
        //        int remaining;
        //        int sendBytes;
        //        NetOutgoingMessage om;
        //        NetSendResult result;

        //        //Send header of stream
        //        if (_currentStreamPosition == 0)
        //        {
        //            //Send stream length and some data at start of stream
        //            remaining = _currentStream.Length - _currentStreamPosition;
        //            sendBytes = (remaining > _headerPacketLenght ? _headerPacketLenght : remaining);
        //            var headerDataBuffer = new byte[sendBytes];
        //            Array.Copy(_currentStream, 0, headerDataBuffer, 0, sendBytes);
        //            _currentStreamPosition += sendBytes;

        //            var streamHeader = new StreamHeader { ContentLength = _currentStream.Length, ContentStart = headerDataBuffer };
        //            var headerData = _serializer.Encode(streamHeader);

        //            om = _client.CreateMessage(sendBytes + 8); //Capacity from Lidgren example
        //            om.Write(headerData);
        //            result = _connection.SendMessage(om, NetDeliveryMethod.ReliableOrdered, Settings.StreamChannel);
        //            //Log.Trace("<-... Send header packet of stream {0}, size {2}, result {1}", _currentStreamToSend.GetHashCode(), result,om.LengthBytes);

        //            //If complete stream fit into header packet
        //            if (_currentStream.Length == _currentStreamPosition)
        //            {
        //                Log.Trace("<-... Short stream {0} has send completely, size {1}", _currentStream.GetHashCode(),
        //                    _currentStream.Length);
        //                _currentStream = null;
        //            }

        //            return;
        //        }

        //        //Get packet from stream
        //        remaining = _currentStream.Length - _currentStreamPosition;
        //        sendBytes = (remaining > _packetLenght ? _packetLenght : remaining);
        //        Array.Copy(_currentStream, _currentStreamPosition, _packetBuffer, 0, sendBytes);
        //        _currentStreamPosition += sendBytes;

        //        //Send packet of stream
        //        om = _client.CreateMessage(sendBytes + 8); //Capacity from Lidgren example
        //        om.Write(_packetBuffer, 0, sendBytes);

        //        result = _connection.SendMessage(om, NetDeliveryMethod.ReliableOrdered, Settings.StreamChannel);
        //        //Log.Trace("<-... Send content packet of stream {0}, size {2}, result {1}", _currentStreamToSend.GetHashCode(), result, om.LengthBytes);

        //        //If complete stream has sended
        //        if (_currentStream.Length == _currentStreamPosition)
        //        {
        //            Log.Trace("<-... Stream {0} has send completely, size {1}", _currentStream.GetHashCode(),
        //                _currentStream.Length);
        //            _currentStream = null;
        //        }
        //    }
        //}

        //private void AddStream([NotNull] byte[] stream)
        //{
        //    if (stream == null) throw new ArgumentNullException("stream");

        //    _streamsToSend.Enqueue(stream);
        //}

        public void Close()
        {
            _connection.Disconnect("Client closed");
            _connection.Tag = null;
        }

        public override string ToString()
        {
            return String.Format("Client {0} to {1}", _id, Connection.RemoteEndPoint);
        }

        private readonly NetConnection _connection;
        private readonly Server _server;
        private readonly int _id;
        private readonly NetPeer _client;
        private readonly MessageSerializer _serializer;
        private readonly BufferBlock<ClientRequest> _clientRequests = new BufferBlock<ClientRequest>();

        private readonly Logger Log;

        /// <summary>
        /// Dispatch message to player controller
        /// </summary>
        /// <param name="msg"></param>
        void IMessageReceiver.Receive(Message msg)
        {
            if (msg is HeroAction)
                HeroActionReceived((HeroAction)msg);            //Call sync
            else if(msg is ClientRequest)
                _clientRequests.Post((ClientRequest)msg);       //Send to queue
            else 
                Log.Error("Received unknown message {0}", msg);
        }
    }
}
