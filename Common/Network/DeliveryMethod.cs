using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Lidgren.Network;
using Silentor.TB.Common.Tools;

namespace Silentor.TB.Common.Network
{
    public struct DeliveryMethod
    {
        public readonly NetDeliveryMethod Method;
        public readonly int Channel;

        public DeliveryMethod(NetDeliveryMethod method, int channel)
        {
            Method = method;
            Channel = channel;
        }

        [Pure]
        public bool IsSameAs([NotNull] NetIncomingMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            return Method == message.DeliveryMethod && Channel == message.SequenceChannel;
        }
    }
}
