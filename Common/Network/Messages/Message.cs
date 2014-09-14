
using Lidgren.Network;

namespace Silentor.TB.Common.Network.Messages
{
    public abstract class Message
    {
        public abstract Headers Header { get; }

        public virtual DeliveryMethod Delivery { get { return Settings.System; } }

        /// <summary>
        /// Serialize message. Base method serializes <see cref="Header"/>
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void Serialize(NetBuffer buffer)
        {
            buffer.Write((byte)Header);
        }

        /// <summary>
        /// Deserialize message. Base method does nothing
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void Deserialize(NetBuffer buffer)
        {
        }

    }

    /// <summary>
    /// C2S message about some player activity, high priority
    /// </summary>
    public abstract class HeroAction : Message
    {
    }

    /// <summary>
    /// Login, Disconnect, etc
    /// </summary>
    public abstract class PlayerManagement : Message
    {
        
    }
}
