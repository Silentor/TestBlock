using Lidgren.Network;
using Silentor.TB.Common.Network.Messages;

namespace Wob.Server.Network
{
    public class OutgoingEnvelop
    {
        public Session Client;
        public Message Message;
        public NetOutgoingMessage SendBuffer;
    }
}