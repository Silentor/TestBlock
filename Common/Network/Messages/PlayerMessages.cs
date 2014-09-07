using System;
using ProtoBuf;

namespace Silentor.TB.Common.Network.Messages
{
    [ProtoContract]
    public class EntityUpdate : Message
    {
        public override Headers Header
        {
            get { return Headers.EntityUpdate; }
        }

        [ProtoMember(1, IsRequired = true)]
        public int Id { get; set; }


        [ProtoMember(2, IsRequired = true)]
        public ProtoVector3 Position { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public ProtoQuaternion Rotation { get; set; }

        [ProtoMember(4)]
        public bool IsRemoved { get; set; }         //True if entity is dissapeared from vision

        public override string ToString()
        {
            if (IsRemoved)
                return string.Format("{0} dissapeared", Id);

            return string.Format("{0} position updated to {1}, rotation to {2}", Id, Position, Rotation);
        }
    }

    [ProtoContract]
    public class PlayerMovement : Message
    {
        public override Headers Header
        {
            get { return Headers.PlayerMovement; }
        }

        [ProtoMember(1, IsRequired = true)]
        public ProtoVector3 Movement { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public ProtoVector2 Rotation { get; set; }

        [ProtoMember(3)]
        public bool Jump { get; set; }

        public override string ToString()
        {
            return String.Format("Movement: {0}, rotation: {1}, jump: {2}", Movement, Rotation, Jump);
        }
    }
}
