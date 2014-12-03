using System;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Common.Network.Serialization
{
    /// <summary>
    /// Mark property <see cref="Message.Header" /> with this attribute/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HeaderAttribute : Attribute
    {
        public Headers Header { get; private set; }

        public HeaderAttribute(Headers header)
        {
            if(header > MessageFactory.MaxHeader) throw new ArgumentOutOfRangeException("header");

            Header = header;
        }
    }
}
