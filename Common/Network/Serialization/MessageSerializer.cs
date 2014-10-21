using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using Lidgren.Network;
using NLog;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Common.Network.Serialization
{
    /// <summary>
    /// Converts buffer of bytes to message
    /// </summary>
    public class MessageSerializer
    {
        public MessageSerializer(Func<int, NetBuffer> bufferGenerator = null, Action<NetBuffer> bufferDisposer = null)
        {
            _bufferGenerator = bufferGenerator;
            _bufferDisposer = bufferDisposer;
        }

        /// <summary>
        /// Used to add custom messages from not current assembly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="header"></param>
        public void AddMessageHeader<T>(Headers header) where T : Message
        {
            _messages[header] = typeof (T);
        }

        /// <summary>
        /// Serialize 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] Serialize(Message message, out int length)
        {
            var netBuffer = GetBuffer(message.Size);
            Serialize(message, netBuffer);

            length = netBuffer.LengthBytes;
            return netBuffer.Data;
        }

        public NetBuffer Serialize(Message message)
        {
            var buffer = GetBuffer(message.Size);
            Serialize(message, buffer);
            return buffer;
        }

        public Message Deserialize(NetBuffer buffer)
        {
            if (!_messagesCollected) ReflectMessagesHeaders();

            var header = (Headers) buffer.ReadByte();
            Type messageType;
            if (_messages.TryGetValue(header, out messageType))
            {
                var constructor = messageType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, 
                    new[]{typeof(NetBuffer)}, null);
                var message = (Message)constructor.Invoke(new[] {(Object)buffer});
                //var message = (Message)Activator.CreateInstance(messageType, true, buffer);
                return message;
            }
            else throw new SerializationException(string.Format("There is no message for header {0}", header));
        }

        public Message Deserialize(byte[] buffer)
        {
            var netBuffer = GetBuffer(buffer.Length);
            netBuffer.Write(buffer);
            return Deserialize(netBuffer);
        }

        private readonly Func<int, NetBuffer> _bufferGenerator;
        private readonly Action<NetBuffer> _bufferDisposer;
        private readonly Dictionary<Headers, Type> _messages = new Dictionary<Headers, Type>();
        private bool _messagesCollected;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private void Serialize(Message message, NetBuffer buffer)
        {
            if (!_messagesCollected) ReflectMessagesHeaders();

            message.Serialize(buffer);
        }

        /// <summary>
        /// Find all messages in current assembly
        /// </summary>
        private void ReflectMessagesHeaders()
        {
            _messagesCollected = true;
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof (Message)))
                {
                    var header = type.GetProperty("Header");
                    var headerAttr = (HeaderAttribute)Attribute.GetCustomAttribute(header, typeof (HeaderAttribute));

                    if (headerAttr == null)
                        throw new SerializationException(string.Format("Message {0} doesnt has Header attribute",
                            type.Name));

                    _messages[headerAttr.Header] = type;
                }
            }

            Log.Debug("Collected {0} message headers", _messages.Count);
        }

        private NetBuffer GetBuffer(int estimatedSize)
        {
            if (_bufferGenerator != null)
                return _bufferGenerator(estimatedSize);
            else
                return new NetBuffer();
        }

        private void DisposeBuffer([NotNull] NetBuffer bufferToDispose)
        {
            if (bufferToDispose == null) throw new ArgumentNullException("bufferToDispose");

            if (_bufferDisposer != null)
                _bufferDisposer(bufferToDispose);
        }
    }
}
