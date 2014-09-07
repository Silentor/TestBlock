using System.Collections.Generic;
using System.Linq;
using Assets.Code.Visualization;
using NLog;
using Silentor.TB.Client;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Meshing;
using Silentor.TB.Client.Tools;
using Silentor.TB.Client.Tools;
using Silentor.TB.Client.Visualization;
using Silentor.TB.Client.Visualization;
using Silentor.TB.Common.Maps;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.Client.Visualization
{
    /// <summary>
    /// Visualize new chunks in map, remove visualization of deleted chunks from map
    /// </summary>
    public class SimpleMapVisualizer : IMapVisualizer
    {
        public SimpleMapVisualizer(IMap map, IChunkletMesher chunkletMesher, IApplicationEvents appEvents, IChunkletViewFactory chunkletFactory)
        {
            _map = map;
            _map.ChunkAdded += MapOnChunkAdded;
            _map.ChunkRemoved += MapOnChunkRemoved;

            _chunkletMesher = chunkletMesher;

            _appEvents = appEvents;
            _chunkletFactory = chunkletFactory;
            _appEvents.FrameTick += AppEventsOnFrameTick;

            _meshWorker = new Worker<UnityApp.ChunkletMeshDataHolder, UnityApp.ChunkletMeshDataHolder>(GenerateMeshThreadWorker);
        }

        private void AppEventsOnFrameTick()
        {
            UnityApp.ChunkletMeshDataHolder chunkletMeshDataHolder;

            while(true)
                if (_meshWorker.GetResult(out chunkletMeshDataHolder))
                {
                    //Update existing chunklet mesh or create new one
                    ChunkletView chunkletView;
                    if (!_currentChunkletViews.TryGetValue(chunkletMeshDataHolder.Chunklet.Position,
                        out chunkletView))
                    {
                        chunkletView = _chunkletFactory.Create(chunkletMeshDataHolder.Chunklet.Position);
                        _currentChunkletViews.Add(chunkletMeshDataHolder.Chunklet.Position, chunkletView);
                    }

                    chunkletView.SetMesh(chunkletMeshDataHolder.Mesh);

                    Log.Trace("Updated mesh for ChunkletView {0}", chunkletMeshDataHolder.Chunklet.Position);

                    //Throttle to one not empty chunklet per frame
                    if(chunkletMeshDataHolder.Mesh.Vertices.Count > 0)
                        return;
                }
                else return;
        }

        private void MapOnChunkRemoved(Chunk chunk)
        {
            if (chunk != null)
                EraseChunk(chunk);
        }

        private void MapOnChunkAdded(Chunk chunk)
        {
            DrawChunk(chunk);

            //Дорисовать недостающие грани у окружающих чанков (если есть таковые)
            foreach (var direction in Vector2i.Directions)
            {
                var nearChunk = chunk.Position + direction;
                if (_map.IsCorrectChunkPosition(nearChunk) && _map.IsChunkPresent(nearChunk))
                {
                    DrawChunk(_map.GetChunk(nearChunk));
                }
            }
        }

        private void DrawChunk(Chunk chunk)
        {
            for (var y = 0; y < Chunk.ChunkletsCount; y++)
            {
                var chunklet = chunk.GetChunklet(y);
                var neighbours = Vector3i.Directions.Select(dir => _map.GetChunklet(chunklet.Position + dir)).ToArray();
                _meshWorker.Add(new UnityApp.ChunkletMeshDataHolder { Chunklet = chunklet, Neighbours = neighbours });
            }

            Log.Trace("Chunk {0} has sent to mesher", chunk.Position);
        }

        private void EraseChunk(Chunk chunk)
        {
            for (var y = 0; y < Chunk.ChunkletsCount; y++)
            {
                ChunkletView chunkletView;
                if (_currentChunkletViews.TryGetValue(new Vector3i(chunk.Position.X, y, chunk.Position.Z),
                    out chunkletView))
                {
                    _chunkletFactory.Dispose(chunkletView);
                    _currentChunkletViews.Remove(new Vector3i(chunk.Position.X, y, chunk.Position.Z));
                }
            }

            Log.Trace("Chunk {0} erased", chunk.Position);
        }

        private UnityApp.ChunkletMeshDataHolder GenerateMeshThreadWorker(UnityApp.ChunkletMeshDataHolder input)
        {
            input.Mesh = _chunkletMesher.Render(input.Chunklet, input.Neighbours);
            return input;
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Worker<UnityApp.ChunkletMeshDataHolder, UnityApp.ChunkletMeshDataHolder> _meshWorker;
        private readonly Dictionary<Vector3i, ChunkletView> _currentChunkletViews = new Dictionary<Vector3i, ChunkletView>();
        private readonly IMap _map;
        private readonly IChunkletMesher _chunkletMesher;
        private readonly IApplicationEvents _appEvents;
        private readonly IChunkletViewFactory _chunkletFactory;
    }
}
