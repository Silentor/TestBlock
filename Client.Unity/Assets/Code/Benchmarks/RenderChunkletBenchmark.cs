using Assets.Code.Visualization;
using Assets.Scripts.Visualization;
using Silentor.TB.Client.Maps;
using Silentor.TB.Client.Meshing;
using Silentor.TB.Common.Maps.Geometry;
using UnityEngine;
using NLog;
using NLog.Config;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps;
using Silentor.TB.Common.Maps.Blocks;

namespace Assets.Code.Benchmarks
{
    public class RenderChunkletBenchmark : MonoBehaviour
    {
        public BlocksAtlas Atlas;

        private SimplestChunkletMesher _renderer;
        private TestGenerator _generator;
        private Map _map;
        private ChunkletViewFactory _chunkletFactory;

        void Awake()
        {
            InitNLog();

            var worldConfig = new WorldConfigStub() {Seed = 666};
            var mapConfig = new MapConfigStub() {Bounds = new Bounds2i(Vector2i.Zero, 100)};
            var blockset = new BlockSet();

            _map = new Map(mapConfig, blockset);
            _renderer = new SimplestChunkletMesher(new BlockMeshers(Atlas, blockset), _map);
            _generator = new TestGenerator(worldConfig, blockset);
            _chunkletFactory = new ChunkletViewFactory(Atlas, new GameObject("Map").AddComponent<MapGizmos>());
        }

        void Start()
        {
            GenerateMap();

            RenderChunk();
        }

        private void InitNLog()
        {
            //Avoid xml configuration to prevent System.Xml overhead
            var logConfig = new LoggingConfiguration();

            LogManager.Configuration = logConfig;
        }

        private void RenderChunk()
        {
            for (int i = 1; i < 7; i++)
            {
                var renderedPos = new Vector3i(0, i, 0);
                var neighbours = new Chunklet[4];

                for (int j = 0; j < neighbours.Length; j++)
                    neighbours[j] = _map.GetChunklet(renderedPos + Vector3i.Directions[j]);

                var meshData = _renderer.Render(_map.GetChunklet(renderedPos), neighbours);

                var chunkletVisual = _chunkletFactory.Create(renderedPos);
                chunkletVisual.SetMesh(meshData);
                
            }
        }

        void GenerateMap()
        {
            _map.SetChunk(new Chunk(_map, _generator.GenerateSync(Vector2i.Zero)));

            foreach (var dir in Vector2i.Cardinals)
                _map.SetChunk(new Chunk(_map, _generator.GenerateSync(dir)));
        }

        private class WorldConfigStub : IGlobeConfig
        {
            public int Seed { get; set; }

            public Bounds2i Bounds { get; private set; }
        }

        private class MapConfigStub : IMapConfig 
        {
            public Bounds2i Bounds { get; set; }
        }
    }
}
