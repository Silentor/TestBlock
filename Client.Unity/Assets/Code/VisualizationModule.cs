using System;
using Assets.Code.Visualization;
using Assets.Code.Visualization.Views;
using Assets.Scripts.Visualization;
using ModestTree.Zenject;
using NLog;
using Silentor.Client.Visualization;
using Silentor.TB.Client;
using Silentor.TB.Client;
using Silentor.TB.Client.Config;
using Silentor.TB.Client.Input;
using Silentor.TB.Client.Meshing;
using Silentor.TB.Client.Visualization;
using Silentor.TB.Client.Visualization;
using Silentor.TB.Client.Visualization.Views;

namespace Silentor.TB.Client
{
    public class VisualizationModule : Installer
    {
        [Inject]
        private readonly Config Settings;

        [Serializable]
        public class Config : IVisualizationConfig
        {
            public BlocksAtlas Blocks;
            public TestPlayerView PlayerPrefab;
            public TestEnemyView EnemyPrefab;
            public MapGizmos MapVisual;

            BlocksAtlas IVisualizationConfig.Blocks { get { return Blocks; } }

            TestPlayerView IVisualizationConfig.PlayerPrefab { get { return PlayerPrefab; } }

            MapGizmos IVisualizationConfig.MapVisual { get{ return MapVisual; } }
        }

        public override void InstallBindings()
        {
            _container.Bind<IClientApp>().ToSingle<UnityApp>();

            //Map visualization
            _container.Bind<IBlocksAtlas>().ToSingle(Settings.Blocks);
            _container.Bind<IBlockMeshers>().ToSingle<BlockMeshers>();
            _container.Bind<SimplestChunkletMesher>().ToSingle();
            _container.Bind<IChunkletViewFactory>().ToSingle<ChunkletViewFactory>();
            _container.Bind<IMapVisualizer>().ToSingle<SimpleMapVisualizer>();
            _container.Bind<IChunkletMesher>().ToSingle<SimplestChunkletMesher>();

            //Actors visualization
            _container.Bind<IActorVisualizer>().ToSingle<SimpleActorVisualizator>();
            _container.Bind<TestPlayerView>().ToSingleFromPrefab<TestPlayerView>(Settings.PlayerPrefab.gameObject);
            _container.Bind<TestEnemyView>().ToTransientFromPrefab<TestEnemyView>(Settings.EnemyPrefab.gameObject);
            _container.Bind<IEnemyViewFactory>().ToSingle<EnemyViewFactory>();

            //Map holder GO
            _container.Bind<MapGizmos>().ToSingleGameObject("Map");
        }
    }
}
