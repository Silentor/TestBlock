using System;
using Silentor.TB.Client.Players;
using Silentor.TB.Client.Config;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Network;
using Silentor.TB.Client.Storage;
using Silentor.TB.Client.Tools;
using Silentor.TB.Common.Maps;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using ModestTree.Zenject;

namespace Silentor.TB.Client
{
    public sealed class GameModule : Installer
    {
        [Inject]
        private readonly Config Settings;

        public void RegisterGameSettings(LoginResponce loginData)
        {
            var mapLoadSize = Math.Min(Settings.ChunkViewRadius + 1, loginData.SimulationSize);
            var mapConfig = new MapConfig(new Bounds2i(Chunk.ToChunkPosition(loginData.Position.ToVector().ToMapPosition()), mapLoadSize));
            Container.Bind<IMapConfig>().ToSingle(mapConfig);

            var playerConfig = new ActorConfig(loginData.Id, loginData.Position.ToVector(), loginData.Rotation.ToQuaternion());
            Container.Bind<ActorConfig>().To(playerConfig).WhenInjectedInto<Player>();
        }

        public Game CreateGame()
        {
            return Container.Resolve<Game>();
        }

        [Serializable]
        public class Config : IGameConfig
        {
            public int ChunkViewRadius = 3;

            public int ChunkCacheSize = 30;

            int IGameConfig.ChunkViewRadius { get { return ChunkViewRadius; } }

            int IGameConfig.ChunkCacheSize { get { return ChunkCacheSize; } }
        }

        public override void InstallBindings()
        {
            Container.Bind<GameModule>().To(this);
            Container.Bind<IGameConfig>().To(Settings);

            Container.Bind<Game>().ToSingle();

            Container.Bind<IWorld>().ToSingle<RemoteWorld>();
            Container.Bind<IMapLoader>().ToSingle<SimpleMapLoader>();
            Container.Bind<IMap>().ToSingle<Map>();
            Container.Bind<IMapEditor>().ToSingle<Map>();

            Container.Bind<IChunkFactory>().ToSingle<ChunkFactory>();
            Container.Bind<IChunkStorage>().ToSingle<ChunkStorage>();

            Container.Bind<IBlockSet>().ToSingle<BlockSet>();

            Container.Bind<IPlayer>().ToSingle<Player>();
            //_container.Bind<IEnemy>().ToTransient<Enemy>();
            Container.Bind<IEnemyFactory>().ToSingle<EnemyFactory>();
            Container.Bind<EnemyFactory.Factory>().ToSingle();              //Not so good
        }
    }
}
