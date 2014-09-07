using System;
using System.Collections.Generic;
using NLog;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Players;
using Silentor.TB.Client.Tools;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Client.Network
{
    public class RemoteWorld : IWorld
    {
        private readonly IGlobeConfig _config;
        private readonly IServer _server;
        private readonly IEnemyFactory _enemyFactory;
        private readonly IPlayer _player;

        private static readonly Logger Log = LogManager.GetLogger("Client.Network.RemoteWorld");
        private readonly List<IActorEditor> _players = new List<IActorEditor>();

        public RemoteWorld(IServer server, IPlayer player, IEnemyFactory enemyFactory)
        {
            _server = server;
            _enemyFactory = enemyFactory;
            _server.ClientConnection.PositionUpdated += ClientConnectionOnPositionUpdated;

            //Add player to actors list
            _players.Add((IActorEditor)player);

            Log.Trace("Started");
        }

        public event Action<IActor> ActorAdded;

        public event Action<IActor> ActorRemoved;

        private void ClientConnectionOnPositionUpdated(EntityUpdate playerPosition)
        {
            var actor = _players.Find(a => a.Id == playerPosition.Id);
            if (actor != null)
            {
                if (playerPosition.IsRemoved)
                {
                    _players.Remove(actor);
                    if (ActorRemoved != null)
                        ActorRemoved(actor);
                }
                else
                {
                    actor.Move(playerPosition.Position.ToVector());
                    actor.Rotate(playerPosition.Rotation.ToQuaternion());
                }
            }
            else
            {
                var enemy = _enemyFactory.Create(new ActorConfig(playerPosition.Id, playerPosition.Position.ToVector(), playerPosition.Rotation.ToQuaternion()));
                _players.Add(enemy);

                if (ActorAdded != null)
                    ActorAdded(enemy);
            }
        }
    }
}
