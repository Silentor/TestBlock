using System.Diagnostics;
using Lidgren.Network;

namespace Silentor.TB.Common.Network.Messages
{
    /// <summary>
    /// Serializable/deserializabe message base class. Deserialization via internal constructor.
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
        /// Hint for serializer, is message needs a compression
        /// </summary>
        public virtual bool Compressible { get { return false; } }

        /// <summary>
        /// Delivery method for networking
        /// </summary>
        public virtual DeliveryMethod Delivery { get { return Settings.System; } }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="buffer"></param>
        internal Message(NetBuffer buffer)
        { }

        protected Message()
        { }

        /// <summary>
        /// Serialize message
        /// </summary>
        /// <param name="buffer"></param>
        internal abstract void Serialize(NetBuffer buffer);
    }

    /// <summary>
    /// C2S message about some hero activity (move, attack), urgent
    /// </summary>
    public abstract class HeroAction : PlayerAction
    {
    }

    /// <summary>
    /// C2S request for some data for client application (map chunk, inventory), not urgent
    /// </summary>
    public abstract class ClientRequest : PlayerAction
    {
    }

    /// <summary>
    /// Login, Disconnect, etc
    /// </summary>
    public abstract class PlayerManagement : Message
    {
    }

    /// <summary>
    /// Some game action from client application
    /// </summary>
    public abstract class PlayerAction : Message
    {
    }
}
