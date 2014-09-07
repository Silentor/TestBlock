using Assets.Code.Visualization.Views;
using NLog;
using Silentor.Client.Visualization;
using Silentor.TB.Client;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Meshing;
using Silentor.TB.Client.Visualization;

namespace Silentor.TB.Client
{
    public class UnityApp : IClientApp
    {
        public UnityApp(IMapVisualizer mapVisualizer, IActorVisualizer actorVisualizer, TestPlayerView playerView)
        {
            _mapVisualizer = mapVisualizer;
            _actorVisualizer = actorVisualizer;
            _playerView = playerView;

            Log.Trace("Created");
        }

        private static readonly Logger Log = LogManager.GetLogger("Client.Visualization.UnityApp");

        private readonly IMapVisualizer _mapVisualizer;
        private readonly IActorVisualizer _actorVisualizer;
        private readonly TestPlayerView _playerView;

        public class ChunkletMeshDataHolder
        {
            //Input
            public Chunklet Chunklet;
            public Chunklet[] Neighbours;

            //Output
            public MeshData Mesh;
        }
    }
}
