using Lidgren.Network;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Server.Network
{
    public class OutgoingEnvelop
    {
        public Client Client;
        public Message Message;
        public NetOutgoingMessage SendBuffer;
    }
}