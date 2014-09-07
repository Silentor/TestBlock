using Lidgren.Network;
using Silentor.TB.Common.Network.Messages;
using Wob.Server.Players;

namespace Wob.Server.Network
{
    public class IncomingEnvelop
    {
        public Simulator Client;
        public Message Message;
        public NetIncomingMessage RecvBuffer;

        public NetConnection Connection 
        { 
            get { return Client != null ? Client.Session.Connection : RecvBuffer.SenderConnection; } 
        }

        public void Recycle()
        {
            Connection.Peer.Recycle(RecvBuffer);
            Client = null;
            Message = null;
            RecvBuffer = null;
        }
    }
}