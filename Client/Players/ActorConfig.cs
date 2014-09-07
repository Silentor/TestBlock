using UnityEngine;

namespace Silentor.TB.Client.Players
{
    public class ActorConfig : IActorConfig
    {
        public ActorConfig(int id, Vector3 position, Quaternion rotation)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
        }

        public int Id { get; private set; }

        public Vector3 Position { get; private set; }

        public Quaternion Rotation { get; private set; }
    }
}