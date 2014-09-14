using System;
using Lidgren.Network;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Network.Messages
{
    //C2S, causes hero creation in world
    public class LoginData : PlayerManagement
    {
        [Header(Headers.Login)]
        public override Headers Header
        {
            get { return Headers.Login; }
        }

        public LoginData(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        internal LoginData()
        {

        }

        public String Name { get; private set; }

        public override void Serialize(NetBuffer buffer)
        {
            base.Serialize(buffer);

            buffer.Write(Name);
        }

        public override void Deserialize(NetBuffer buffer)
        {
            base.Deserialize(buffer);

            Name = buffer.ReadString();
        }
    }

    //S2C, simulated hero created, player can play
    public class LoginResponce : PlayerManagement
    {
        [Header(Headers.LoginResponce)]
        public override Headers Header
        {
            get { return Headers.LoginResponce; }
        }

        public LoginResponce(int id, ProtoVector3 position, ProtoQuaternion rotation, int simulationSize)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            SimulationSize = simulationSize;
        }

                /// <summary>
        /// Deserialization
        /// </summary>
        internal LoginResponce()
        {

        }

        public int Id { get; private set; }

        public ProtoVector3 Position { get; private set; }

        public ProtoQuaternion Rotation { get; private set; }

        public int SimulationSize { get; private set; }

        public override void Serialize(NetBuffer buffer)
        {
            base.Serialize(buffer);

            buffer.Write(Id);
            buffer.Write(Position);
            buffer.Write(Rotation);
            buffer.Write(SimulationSize);
        }

        public override void Deserialize(NetBuffer buffer)
        {
            base.Deserialize(buffer);

            Id = buffer.ReadInt32();
            Position = buffer.ReadVector3();
            Rotation = buffer.ReadQuaternion();
            SimulationSize = buffer.ReadInt32();
        }
    }

    //Server internal, connection to client is lost, remove hero from simulated world
    public class Disconnect : PlayerManagement
    {
        [Header(Headers.Disconnect)]
        public override Headers Header
        {
            get { return Headers.Disconnect; }
        }
    }
}