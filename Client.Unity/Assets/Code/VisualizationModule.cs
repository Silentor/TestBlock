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
            Container.Bind<IClientApp>().ToSingle<UnityApp>();

            //Map visualization
            Container.Bind<IBlocksAtlas>().ToSingle(Settings.Blocks);
            Container.Bind<IBlockMeshers>().ToSingle<BlockMeshers>();
            Container.Bind<SimplestChunkletMesher>().ToSingle();
            Container.Bind<IChunkletViewFactory>().ToSingle<ChunkletViewFactory>();
            Container.Bind<IMapVisualizer>().ToSingle<SimpleMapVisualizer>();
            Container.Bind<IChunkletMesher>().ToSingle<SimplestChunkletMesher>();

            //Actors visualization
            Container.Bind<IActorVisualizer>().ToSingle<SimpleActorVisualizator>();
            Container.Bind<TestPlayerView>().ToSingleFromPrefab<TestPlayerView>(Settings.PlayerPrefab.gameObject);
            Container.Bind<TestEnemyView>().ToTransientFromPrefab<TestEnemyView>(Settings.EnemyPrefab.gameObject);
            Container.Bind<IEnemyViewFactory>().ToSingle<EnemyViewFactory>();

            //Map holder GO
            Container.Bind<MapGizmos>().ToSingleGameObject("Map");
        }
    }
}
