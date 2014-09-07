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
            _container.Bind<IMapConfig>().ToSingle(mapConfig);

            var playerConfig = new ActorConfig(loginData.Id, loginData.Position.ToVector(), loginData.Rotation.ToQuaternion());
            _container.Bind<ActorConfig>().To(playerConfig).WhenInjectedInto<Player>();
        }

        public Game CreateGame()
        {
            return _container.Resolve<Game>();
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
            _container.Bind<GameModule>().To(this);
            _container.Bind<IGameConfig>().To(Settings);

            _container.Bind<Game>().ToSingle();

            _container.Bind<IWorld>().ToSingle<RemoteWorld>();
            _container.Bind<IMapLoader>().ToSingle<SimpleMapLoader>();
            _container.Bind<IMap>().ToSingle<Map>();
            _container.Bind<IMapEditor>().ToSingle<Map>();

            _container.Bind<IChunkFactory>().ToSingle<ChunkFactory>();
            _container.Bind<IChunkStorage>().ToSingle<ChunkStorage>();

            _container.Bind<IBlockSet>().ToSingle<BlockSet>();

            _container.Bind<IPlayer>().ToSingle<Player>();
            //_container.Bind<IEnemy>().ToTransient<Enemy>();
            _container.Bind<IEnemyFactory>().ToSingle<EnemyFactory>();
        }
    }
}
