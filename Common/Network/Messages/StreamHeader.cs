using ProtoBuf;

namespace Silentor.TB.Common.Network.Messages
{
    /// <summary>
    /// Used to send big chunks of data
    /// </summary>
    [ProtoContract]
    public class StreamHeader : Message
    {
        public override Headers Header
        {
            get { return Headers.StreamHeader; }
        }

        [ProtoMember(1, IsRequired = true)]
        public int ContentLength { get; set; }

        /// <summary>
        /// To transfer a some start data of stream
        /// </summary>
        [ProtoMember(2, IsPacked = true, OverwriteList = true, IsRequired = true)]
        public byte[] ContentStart { get; set; }

    }
}