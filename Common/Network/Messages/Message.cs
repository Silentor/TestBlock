using Lidgren.Network;

namespace Silentor.TB.Common.Network.Messages
{
    /// <summary>
    /// Serializable/deserializabe message base class. Deserialization via constructor.
    /// Based on Lidgren NetBuffer
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Message type
        /// </summary>
        public abstract Headers Header { get; }

        /// <summary>
        /// Estimated message size in bytes (for buffer allocation optimization)
        /// </summary>
        public abstract int Size { get; }

        /// <summary>
        /// Delivery method for networking
        /// </summary>
        public virtual DeliveryMethod Delivery { get { return Settings.System; } }

        /// <summary>
        /// Serialize message. Base method serializes <see cref="Header"/> only
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void Serialize(NetBuffer buffer)
        {
            buffer.Write((byte)Header);
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
