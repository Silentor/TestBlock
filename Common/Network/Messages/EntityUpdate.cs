using Lidgren.Network;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Network.Messages
{
    public class EntityUpdate : Message
    {
        public EntityUpdate(int id, ProtoVector3 position, ProtoQuaternion rotation, bool isRemoved)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            IsRemoved = isRemoved;
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        internal EntityUpdate()
        {
            
        }

        [Header(Headers.EntityUpdate)]
        public override Headers Header
        {
            get { return Headers.EntityUpdate; }
        }

        public int Id { get; private set; }

        public ProtoVector3 Position { get; private set; }

        public ProtoQuaternion Rotation { get; private set; }

        public bool IsRemoved { get; private set; }         //True if entity is dissapeared from vision

        public override void Serialize(NetBuffer buffer)
        {
            base.Serialize(buffer);

            buffer.Write(Id);
            buffer.Write(Position);
            buffer.Write(Rotation);
            buffer.Write(IsRemoved);
        }

        public override void Deserialize(NetBuffer buffer)
        {
            base.Deserialize(buffer);

            Id = buffer.ReadInt32();
            Position = buffer.ReadVector3();
            Rotation = buffer.ReadQuaternion();
            IsRemoved = buffer.ReadBoolean();
        }

        public override string ToString()
        {
            if (IsRemoved)
                return string.Format("{0} dissapeared", Id);

            return string.Format("{0} position updated to {1}, rotation to {2}", Id, Position, Rotation);
        }
    }
}