using System;
using System.Collections.Concurrent;
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
    public class Session
    {
        private readonly NetConnection _connection;
        private readonly Server _server;
        private readonly NetPeer _client;
        private readonly MessageSerializer _serializer;

        //Streaming send stuff
        private readonly ConcurrentQueue<byte[]> _streamsToSend = new ConcurrentQueue<byte[]>();
        private byte[] _currentStream;
        private int _currentStreamPosition;
        private readonly int _packetLenght;
        private readonly int _headerPacketLenght;
        private readonly byte[] _packetBuffer;

        private static int _clientCounter = 0;
        private readonly Logger Log;

        public NetConnection Connection { get { return _connection; } }

        public Session([NotNull] NetConnection clientConnection, Server server, int id)
        {
            if (clientConnection == null) throw new ArgumentNullException("clientConnection");

            _connection = clientConnection;
            _server = server;
            _client = _connection.Peer;

            _packetLenght = _connection.Peer.Configuration.MaximumTransmissionUnit - 20;
            _headerPacketLenght = _packetLenght - 10;           //Зарезервировать немного места под заголовочную информацию

            _packetBuffer = new byte[_packetLenght];
            _serializer = new MessageSerializer(() => _client.CreateMessage());

            Log = LogManager.GetLogger("Wob.Server.Network.Session" + id);
            Log.Debug("Created session");
        }

        public void LoginAccept(int id, Vector3 startPosition, Quaternion rotation, int simulationSize, Simulator simulator)
        {
            var message = new LoginResponce(id, startPosition.ToProtoVector(), rotation.ToProtoQuaternion(), 
                simulationSize);
            _connection.Tag = simulator;
            _server.Send(message, this);

            Log.Info("<-... Send login acceptation for {0}", id);
        }

        public void SendPosition(int id, Vector3 position, Quaternion rotation, bool isRemoved = false)
        {
            var playerPosition = new EntityUpdate(id, position.ToProtoVector(), rotation.ToProtoQuaternion(), isRemoved);
            _server.Send(playerPosition, this);

            Log.Info("<-... Send new position {0}", playerPosition);
        }

        /// <summary>
        /// Send chunk content
        /// </summary>
        /// <param name="chunkData"></param>
        public void SendChunk(ChunkContents chunkData)
        {
            _server.Send(new ChunkMessage(chunkData), this);
            Log.Info("<-... Send chunk {0}", chunkData.Position);
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

        public NetOutgoingMessage CreateSendBuffer(int size)
        {
            return _client.CreateMessage(size);
        }

        public NetOutgoingMessage CreateSendBuffer()
        {
            return _client.CreateMessage();
        }


        public void Close()
        {
            _connection.Disconnect("Client closed");
        }

        public override string ToString()
        {
            return String.Format("Session from {0}", Connection.RemoteEndPoint);
        }
    }
}
