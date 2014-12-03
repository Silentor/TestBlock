using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Network.Messages
{
    public class TestMessageBase : Message
    {
        [Header(Headers.Test)]
        public override Headers Header
        {
            get { return Headers.Test; }
        }

        public override int Size
        {
            get { return 0; }
        }

        internal override void Serialize(NetBuffer buffer)
        {
            SerializeTest(buffer);
        }

        //To override in test messages
        protected virtual void SerializeTest(NetBuffer buffer)
        {
            
        }
    }
}
