using Lidgren.Network;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Network.Messages
{
    public class ChunkRequestMessage : ClientRequest
    {
        public ChunkRequestMessage(Vector2i position)
        {
            Position = position;
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        internal ChunkRequestMessage(NetBuffer buffer)
        {
            Position = buffer.ReadVector2i();
        }

        public Vector2i Position { get; private set; }

        [Header(Headers.GetChunk)]
        public override Headers Header
        {
            get { return Headers.GetChunk; }
        }

        public override int Size
        {
            get { return 1 + 8; }
        }

        public override void Serialize(NetBuffer buffer)
        {
            base.Serialize(buffer);             //1

            buffer.Write(Position);             //8
        }
    }
}