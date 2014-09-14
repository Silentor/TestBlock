using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using Lidgren.Network;
using NLog;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Common.Network.Serialization
{
    public class MessageSerializer
    {
        public MessageSerializer(Func<NetBuffer> bufferPool = null)
        {
            _bufferPool = bufferPool;
        }

        public void AddMessageHeader<T>(Headers header) where T : Message
        {
            _messages[header] = typeof (T);
        }

        public void Serialize(Message message, NetBuffer buffer)
        {
            if (!_messagesCollected) ReflectMessagesHeaders();

            message.Serialize(buffer);
        }

        public byte[] Serialize(Message message, out int length)
        {
            var netBuffer = GetBuffer();
            Serialize(message, netBuffer);

            length = netBuffer.LengthBytes;
            return netBuffer.Data;
        }

        public NetBuffer Serialize(Message message)
        {
            var buffer = GetBuffer();
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
                //var cons = messageType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic);
                var message = (Message)Activator.CreateInstance(messageType, true);
                message.Deserialize(buffer);
                return message;
            }
            else throw new SerializationException(string.Format("There is no message for header {0}", header));
        }

        public Message Deserialize(byte[] buffer)
        {
            var netBuffer = GetBuffer();
            netBuffer.Write(buffer);
            return Deserialize(netBuffer);
        }

        private readonly Func<NetBuffer> _bufferPool;
        private readonly Dictionary<Headers, Type> _messages = new Dictionary<Headers, Type>();
        private bool _messagesCollected;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

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

        private NetBuffer GetBuffer()
        {
            if (_bufferPool != null)
                return _bufferPool();
            else
                return new NetBuffer();
        }
    }
}
