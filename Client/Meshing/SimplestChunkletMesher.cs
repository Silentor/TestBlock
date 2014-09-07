using System.Linq;
using NLog;
using Silentor.TB.Client.Maps;
using Silentor.TB.Common.Maps;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Tools;
using UnityEngine;

namespace Silentor.TB.Client.Meshing
{
    /// <summary>
    /// Naive generation of MeshData for Chunlket
    /// </summary>
    public class SimplestChunkletMesher : IChunkletMesher
    {
        public SimplestChunkletMesher(IBlockMeshers blockMeshers, IMap map)
        {
            _map = map;

            //Cache block BlockMeshers
            _meshers = new SimplestBlockMesher[256];
            foreach (var renderer in blockMeshers)
                _meshers[renderer.Block.Id] = renderer;
        }

        public MeshData Render(Chunklet chunklet, Chunklet[] neighbours)
        {
            //Simplest chunklet mesher cant process chunklets without neighbour
            if (neighbours.Any(n => n == null))
                return MeshData.Empty;

            ChunkletBuildTimer.StartTimer();

            var meshData = new MeshData(3);

            for (var z = 0; z < Chunklet.SizeZ; z++)
                for (var x = 0; x < Chunklet.SizeX; x++)
                    for (var y = 0; y < Chunklet.SizeY; y++)
                    {
                        var blockPos = Chunklet.ToWorldPosition(chunklet.Position, new Vector3i(x, y, z));
                        var block = _map.GetBlockData(blockPos);
                        if (block.IsBlockVisible)
                        {
                            var renderer = _meshers[block.Id];
                            renderer.Render(new Vector3(x, y, z), meshData, GetVisibleFaces(blockPos, block, _map));
                        }
                    }

            Log.Trace("Rendered chunklet {0}, vertices: {1}, avgtime: {2}", chunklet, meshData.Vertices.Count, ChunkletBuildTimer.StopTimer().AverageTime);

            return meshData;
        }

        private readonly IMap _map;
        private readonly SimplestBlockMesher[] _meshers;
        private readonly AverageTimer ChunkletBuildTimer = new AverageTimer();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private int GetVisibleFaces(Vector3i blockPos, BlockData block, IMap map)
        {
            var result = 0;
            for (var i = 0; i < Vector3i.Directions.Length; i++)
            {
                var direction = Vector3i.Directions[i];
                var nearBlock = map.GetBlockData(blockPos + direction);
                if (nearBlock.IsTransparent && nearBlock.Id != block.Id)
                    result |= 1 << i;
            }

            return result;
        }
    }
}
