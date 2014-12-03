using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Lidgren.Network;
using NLog;
using Silentor.TB.Common.Network.Compression;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Common.Network.Serialization
{
    /// <summary>
    /// Converts buffer of bytes -- message. Byte buffer always precedes by header.
    /// Header byte: c nnnnnnn, where c - is buffer compressed bit, nnnnnnn - header
    /// </summary>
    public class MessageSerializer
    {
        public const byte HeaderSize = 1;

        public MessageSerializer(MessageFactory messageFactory)
        {
            _messageFactory = messageFactory;
        }

        /// <summary>
        /// Serialize message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="length"></param>
        /// <param name="compress"></param>
        /// <returns></returns>
        public byte[] Serialize(Message message, out int length, bool? compress = null)
        {
            var buffer = GetBuffer(message.Size + HeaderSize);
            Serialize(message, buffer, compress);
            length = buffer.LengthBytes;
            return buffer.Data;
        }

        public T Serialize<T>(Message message, T toBuffer, bool? compress = null) where T : NetBuffer
        {
            if (compress == true || (compress == null && message.Compressible))
            {
                var tempBuffer = GetBuffer(message.Size + HeaderSize);
                WriteHeader(false, message.Header, tempBuffer);
                message.Serialize(tempBuffer);

                var compressed = LZ4Wrapper.Wrap(tempBuffer.Data, 0, tempBuffer.LengthBytes);

                WriteHeader(true, message.Header, toBuffer);
                toBuffer.Write(compressed);
            }
            else
            {
                WriteHeader(false, message.Header, toBuffer);
                message.Serialize(toBuffer);
            }

            return toBuffer;
        }

        public Message Deserialize(NetBuffer buffer)
        {
            bool isCompressed;
            var header = ReadHeader(out isCompressed, buffer);

            Message message;
            if (isCompressed)
            {
                var compressed = buffer.ReadBytes(buffer.LengthBytes - HeaderSize);
                var uncompressed = LZ4Wrapper.Unwrap(compressed);
                var uncompressedBuffer = GetBuffer(uncompressed.Length);
                uncompressedBuffer.Write(uncompressed);

                header = ReadHeader(out isCompressed, uncompressedBuffer);
                Debug.Assert(!isCompressed, "Double compression is not permitted");

                message = _messageFactory.Create(header, uncompressedBuffer);
            }
            else
                message = _messageFactory.Create(header, buffer);

            return message;
        }

        public Message Deserialize(byte[] buffer)
        {
            var netBuffer = GetBuffer(buffer.Length);
            netBuffer.Write(buffer);

            return Deserialize(netBuffer);
        }

        private readonly MessageFactory _messageFactory;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static void WriteHeader(bool isCompressed, Headers header, NetBuffer toBuffer)
        {
            toBuffer.Write(isCompressed);
            toBuffer.Write((byte)header, 7);
        }

        private static Headers ReadHeader(out bool isCompressed, NetBuffer fromBuffer)
        {
            isCompressed = fromBuffer.ReadBoolean();
            return (Headers) fromBuffer.ReadByte(7);
        }

        private NetBuffer GetBuffer(int estimatedSize)
        {
            return new NetBuffer {Data = new byte[estimatedSize]};
        }
    }
}
