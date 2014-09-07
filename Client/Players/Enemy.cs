using Silentor.TB.Client.Players;
using UnityEngine;

namespace Silentor.TB.Client.Players
{
    public class Enemy : Actor, IEnemy
    {
        public Enemy(ActorConfig config)
            : base(config)
        {
        }
    }
}