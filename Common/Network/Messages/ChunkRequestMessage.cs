using Lidgren.Network;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Network.Messages
{
    public class ChunkRequestMessage : Message
    {
        public ChunkRequestMessage(Vector2i position)
        {
            Position = position;
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        internal ChunkRequestMessage()
        {
            
        }

        [Header(Headers.GetChunk)]
        public override Headers Header
        {
            get { return Headers.GetChunk; }
        }

        public Vector2i Position { get; private set; }

        public override void Serialize(NetBuffer buffer)
        {
            base.Serialize(buffer);

            buffer.Write(Position);
        }

        public override void Deserialize(NetBuffer buffer)
        {
            base.Deserialize(buffer);

            Position = buffer.ReadVector2i();
        }
    }
}