using ProtoBuf;

namespace Silentor.TB.Common.Network.Messages
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(50, typeof(LoginData))]
    [ProtoInclude(51, typeof(LoginResponce))]
    [ProtoInclude(52, typeof(ChunkRequestMessage))]
    [ProtoInclude(53, typeof(ChunkContents))]
    [ProtoInclude(54, typeof(StreamHeader))]
    [ProtoInclude(55, typeof(EntityUpdate))]
    [ProtoInclude(56, typeof(PlayerMovement))]
    public abstract partial class Message
    {
        public abstract Headers Header { get; }

        public virtual DeliveryMethod Delivery { get { return Settings.System; } }
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
    public abstract class HeroManagement : Message
    {
    }
}
