using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silentor.TB.Common.Network.Messages
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
            Header = header;
        }
    }
}
