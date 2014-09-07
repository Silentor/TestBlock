using ProtoBuf;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Common.Network.Messages
{
    [ProtoContract]
    public class ChunkRequestMessage : Message
    {
        public override Headers Header
        {
            get { return Headers.GetChunk; }
        }

        [ProtoMember(1, IsRequired = true)]
        public Vector2i Position { get; set; }
    }
}