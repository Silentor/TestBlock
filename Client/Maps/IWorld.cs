using System;
using Silentor.TB.Client.Players;

namespace Silentor.TB.Client.Maps
{
    public interface IWorld
    {
        event Action<IActor> ActorAdded;

        event Action<IActor> ActorRemoved;
    }
}

