using System;
using Silentor.TB.Client.Players;
using UnityEngine;

namespace Silentor.TB.Client.Players
{
    public class Actor : IActor, IActorEditor
    {
        public int Id { get; private set; }

        public Vector3 Position { get; private set; }

        public Quaternion Rotation { get; private set; }

        public event Action Moved;

        public event Action Rotated;

        public Actor(IActorConfig config)
        {
            Id = config.Id;
            Position = config.Position;
            Rotation = config.Rotation;
        }

        #region Actor Editor

        void IActorEditor.Move(Vector3 newPosition)
        {
            Position = newPosition;
            if (Moved != null)
                Moved();
        }

        void IActorEditor.Rotate(Quaternion newRotation)
        {
            Rotation = newRotation;
            if (Rotated != null)
                Rotated();
        }

        #endregion
    }
}
