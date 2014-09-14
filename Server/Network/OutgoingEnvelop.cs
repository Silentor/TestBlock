using Lidgren.Network;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Server.Network
{
    public class OutgoingEnvelop
    {
        public Session Client;
        public Message Message;
        public NetOutgoingMessage SendBuffer;
    }
}