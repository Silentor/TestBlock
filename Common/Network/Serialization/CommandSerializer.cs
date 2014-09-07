#define COMPRESSION

using System.Diagnostics;
using System.IO;
using NLog;
using ProtoBuf;
using Silentor.TB.Common.Network.Compression;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Common.Network.Serialization
{
    /// <summary>
    /// Protobuf universal serialier. Not thread safe
    /// </summary>
    public class CommandSerializer
    {
        /// <summary>
        /// Serialize message to data buffer. Not thread safe. Size of buffer can be bigger than data itself, 
        /// see <param name="length"></param>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="compress"></param>
        /// <returns></returns>
        public byte[] Encode<T>(T data, out int length, bool compress = false) where T : Message
        {
            if (Log.IsTraceEnabled)
            {
                _serializatorTimer.Reset();
                _serializatorTimer.Start();
            }

            _encodeBuffer.Position = 0;
            Serializer.Serialize(_encodeBuffer, data);

            byte[] result;
#if COMPRESSION
            if (compress)
            {
                result = LZ4Encrypter.Wrap(_encodeBuffer.GetBuffer(), 0, (int) _encodeBuffer.Position);
                length = result.Length;
            }
            else
            {
                result = _encodeBuffer.GetBuffer();
                length = (int) _encodeBuffer.Position;
            }
#else
            result = _encodeBuffer.GetBuffer();
            length = (int) _encodeBuffer.Position;
#endif

            if (Log.IsTraceEnabled)
            {
                _serializatorTimer.Stop();
#if COMPRESSION
                if (compress)
                    Log.Trace("Command {0} serilalization time: {1} mks, length {2}, compressed {3}",
                        data.GetType().Name, _serializatorTimer.ElapsedTicks/1000, _encodeBuffer.Position, length);
                else
                    Log.Trace("Command {0} serilalization time: {1} mks, length {2}", data.GetType().Name,
                        _serializatorTimer.ElapsedTicks/1000, length);
#else
                Log.Trace("Command {0} serilalization time: {1} mks, length {2}", data.GetType().Name,
                        _serializatorTimer.ElapsedTicks/1000, length);
#endif
            }

            return result;
        }

        /// <summary>
        /// Deserialize message from data buffer. Not thread safe
        /// </summary>
        /// <param name="data"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public Message Decode(byte[] data, bool compressed = false)
        {
#if COMPRESSION
            if (compressed)
                data = LZ4Encrypter.Unwrap(data);
#endif
            _decodeBuffer.SetLength(0);
            _decodeBuffer.Write(data, 0, data.Length);
            _decodeBuffer.Position = 0;
            var result = Serializer.Deserialize<Message>(_decodeBuffer);

            return result;
        }

        private readonly MemoryStream _encodeBuffer = new MemoryStream();
        private readonly MemoryStream _decodeBuffer = new MemoryStream();
        private readonly Stopwatch _serializatorTimer = new Stopwatch();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    }
}
