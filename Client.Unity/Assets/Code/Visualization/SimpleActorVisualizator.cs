using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Code.Visualization;
using Assets.Code.Visualization.Views;
using NLog;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Players;
using Silentor.TB.Client.Visualization;
using Silentor.TB.Client.Visualization.Views;

namespace Silentor.TB.Client.Visualization
{
    /// <summary>
    /// Create gameobjects for added actors, remove gameobjects for removed actors
    /// </summary>
    public class SimpleActorVisualizator : IActorVisualizer
    {
        public SimpleActorVisualizator(IWorld world,  IEnemyViewFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;

            _world = world;
            _world.ActorAdded += GameOnActorAdded;
            _world.ActorRemoved += GameOnActorRemoved;
        }

        private void GameOnActorRemoved(IActor actor)
        {
            var removed = _enemies.FindIndex(e => e.Id == actor.Id);
            _enemies[removed].Destroy();
            _enemies.RemoveAt(removed);

            Log.Info("Deleted view for actor {0}", actor.Id);
        }

        private void GameOnActorAdded(IActor actor)
        {
            //Enemy view is self updateable
            _enemies.Add(_enemyFactory.Create(actor));

            Log.Info("Created view for actor {0}", actor.Id);
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly List<TestEnemyView> _enemies = new List<TestEnemyView>();
        private readonly IWorld _world;
        private readonly IEnemyViewFactory _enemyFactory;
    }
}
