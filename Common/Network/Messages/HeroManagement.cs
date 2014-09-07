using System;
using ProtoBuf;

namespace Silentor.TB.Common.Network.Messages
{
    //C2S, causes hero creation in world
    [ProtoContract(SkipConstructor = true)]
    public class LoginData : HeroManagement
    {
        public override Headers Header
        {
            get { return Headers.Login; }
        }

        public LoginData(string name)
        {
            Name = name;
        }

        [ProtoMember(1, IsRequired = true)]
        public String Name { get; private set; }
    }

    //S2C, simulated hero created, player can play
    [ProtoContract(SkipConstructor = true)]
    public class LoginResponce : HeroManagement
    {
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

        [ProtoMember(1, IsRequired = true)]
        public int Id { get; private set; }

        [ProtoMember(2, IsRequired = true)]
        public ProtoVector3 Position { get; private set; }

        [ProtoMember(3, IsRequired = true)]
        public ProtoQuaternion Rotation { get; private set; }

        [ProtoMember(4, IsRequired = true)]
        public int SimulationSize { get; private set; }
    }

    //Server internal, connection to client is lost, remove hero from simulated world
    public class Disconnect : HeroManagement
    {
        public override Headers Header
        {
            get { return Headers.Disconnect; }
        }
    }
}