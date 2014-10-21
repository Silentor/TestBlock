using Lidgren.Network;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Server.Players;

namespace Silentor.TB.Server.Network
{
    public class IncomingEnvelop
    {
        public HeroController Hero;
        public Message Message;
        public NetIncomingMessage RecvBuffer;

        public NetConnection Connection 
        { 
            get { return RecvBuffer.SenderConnection; } 
        }

        public void Recycle()
        {
            Connection.Peer.Recycle(RecvBuffer);
            Hero = null;
            Message = null;
            RecvBuffer = null;
        }
    }
}