using System;
using Lidgren.Network;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Network.Messages
{
    public class HeroMovement : HeroAction
    {
        public HeroMovement(ProtoVector3 movement, ProtoVector2 rotation, bool jump)
        {
            Movement = movement;
            Rotation = rotation;
            Jump = jump;
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        internal HeroMovement(NetBuffer buffer)
        {
            Movement = buffer.ReadVector3();
            Rotation = buffer.ReadVector2();
            Jump = buffer.ReadBoolean();
        }

        public ProtoVector3 Movement { get; private set; }

        public ProtoVector2 Rotation { get; private set; }

        public bool Jump { get; private set; }

        [Header(Headers.HeroMovement)]
        public override Headers Header
        {
            get { return Headers.HeroMovement; }
        }

        public override int Size
        {
            get { return 12 + 16 + 1; }
        }

        internal override void Serialize(NetBuffer buffer)
        {
            buffer.Write(Movement);             //12
            buffer.Write(Rotation);             //16
            buffer.Write(Jump);                 //1
        }

        public override string ToString()
        {
            return String.Format("Movement: {0}, rotation: {1}, jump: {2}", Movement, Rotation, Jump);
        }
    }
}
