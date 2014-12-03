using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Lidgren.Network;
using NLog;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Common.Network.Serialization
{
    /// <summary>
    /// Create message by its header
    /// </summary>
    public class MessageFactory
    {
        public MessageFactory()
        {
            PopulateMessageList();
        }

        /// <summary>
        /// Create message and load it from buffer
        /// </summary>
        /// <param name="header"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        internal Message Create(Headers header, NetBuffer buffer)
        {
            var constructor = _messages[header];
            var message = (Message)constructor.Invoke(new[] { (Object)buffer });
            return message;
        }

#if DEBUG
        /// <summary>
        /// Add custom message not from current assembly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddCustomMessage<T>() where T : Message
        {
            var headerProp = typeof(T).GetProperty("Header");
            var headerAttr = (HeaderAttribute)Attribute.GetCustomAttribute(headerProp, typeof(HeaderAttribute));

            if (headerAttr == null)
                throw new SerializationException(String.Format("Message {0} doesnt has Header attribute",
                    typeof(T).Name));

            var header = headerAttr.Header;
            if (header > MaxHeader) throw new InvalidOperationException("Message header is out of range");

            var constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                new[] { typeof(NetBuffer) }, null);

            if (constructor != null)
                _messages[header] = constructor;
            else
                throw new InvalidOperationException(String.Format(
                    "Message {0} did nit have deserialization constructor", typeof (T).Name));
        }
#endif

        private readonly Dictionary<Headers, ConstructorInfo> _messages = new Dictionary<Headers, ConstructorInfo>();
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Find all messages in current assembly
        /// </summary>
        private void PopulateMessageList()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Message)))
                {
                    var header = type.GetProperty("Header");
                    var headerAttr = (HeaderAttribute)Attribute.GetCustomAttribute(header, typeof(HeaderAttribute));

                    if (headerAttr == null)
                        throw new SerializationException(String.Format("Message {0} doesnt has Header attribute",
                            type.Name));

                    var messageContructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                        new[] { typeof(NetBuffer) }, null);

                    _messages[headerAttr.Header] = messageContructor;
                }
            }

            Log.Debug("Collected {0} message headers", _messages.Count);
        }

        public const Headers MaxHeader = (Headers) 127;
    }
}
