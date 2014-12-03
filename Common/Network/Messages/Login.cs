using System;
using System.Text;
using JetBrains.Annotations;
using Lidgren.Network;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Network.Messages
{
    //C2S, causes hero creation in world
    public class LoginData : PlayerManagement
    {
        public LoginData([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            Name = name;
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        internal LoginData(NetBuffer buffer)
        {
            Name = buffer.ReadString();
        }

        public String Name { get; private set; }

        [Header(Headers.Login)]
        public override Headers Header
        {
            get { return Headers.Login; }
        }

        public override int Size
        {
            get { return Encoding.UTF8.GetByteCount(Name); }
        }

        internal override void Serialize(NetBuffer buffer)
        {
            buffer.Write(Name);                     //?
        }
    }

    //S2C, simulated hero created, player can play
    public class LoginResponce : PlayerManagement
    {
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
        internal LoginResponce(NetBuffer buffer)
        {
            Id = buffer.ReadInt32();
            Position = buffer.ReadVector3();
            Rotation = buffer.ReadQuaternion();
            SimulationSize = buffer.ReadInt32();
        }

        /// <summary>
        /// Id of player
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Start position of player
        /// </summary>
        public ProtoVector3 Position { get; private set; }

        /// <summary>
        /// Start rotation of player
        /// </summary>
        public ProtoQuaternion Rotation { get; private set; }

        /// <summary>
        /// Allowed size of Map around player
        /// </summary>
        public int SimulationSize { get; private set; }

        [Header(Headers.LoginResponce)]
        public override Headers Header
        {
            get { return Headers.LoginResponce; }
        }

        public override int Size
        {
            get { return 4 + 12 + 16 + 4; }
        }

        internal override void Serialize(NetBuffer buffer)
        {
            buffer.Write(Id);                       //4 
            buffer.Write(Position);                 //12
            buffer.Write(Rotation);                 //16
            buffer.Write(SimulationSize);           //4
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

        public override int Size
        {
            get { return 0; }
        }

        internal override void Serialize(NetBuffer buffer)
        {
            //Empty            
        }
    }
}