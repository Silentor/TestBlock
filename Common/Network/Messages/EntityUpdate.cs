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
        internal EntityUpdate(NetBuffer buffer)
        {
            Id = buffer.ReadInt32();
            Position = buffer.ReadVector3();
            Rotation = buffer.ReadQuaternion();
            IsRemoved = buffer.ReadBoolean();
        }

        public int Id { get; private set; }

        public ProtoVector3 Position { get; private set; }

        public ProtoQuaternion Rotation { get; private set; }

        public bool IsRemoved { get; private set; }

        [Header(Headers.EntityUpdate)]
        public override Headers Header
        {
            get { return Headers.EntityUpdate; }
        }

        public override int Size
        {
            get { return 1 + 4 + 12 + 16 + 1; }
        }

        //True if entity is dissapeared from vision

        public override void Serialize(NetBuffer buffer)
        {
            base.Serialize(buffer);             //1

            buffer.Write(Id);                   //4 
            buffer.Write(Position);             //12
            buffer.Write(Rotation);             //16
            buffer.Write(IsRemoved);            //1
        }

        public override string ToString()
        {
            if (IsRemoved)
                return string.Format("{0} dissapeared", Id);

            return string.Format("{0} position updated to {1}, rotation to {2}", Id, Position, Rotation);
        }
    }
}