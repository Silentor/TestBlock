using System;
using UnityEngine;

namespace Silentor.TB.Client.Players
{
    public interface IActor : IActorConfig
    {
        event Action Moved;

        event Action Rotated;
    }

    public interface IActorConfig
    {
        int Id { get; }

        Vector3 Position { get; }

        Quaternion Rotation { get; }
    }

    public interface IActorEditor : IActor
    {
        void Move(Vector3 position);

        void Rotate(Quaternion rotation);
    }
}