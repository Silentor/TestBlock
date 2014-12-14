using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Common.Tools;
using Silentor.TB.Server.Maps.Voronoi;

namespace Silentor.TB.Server.Maps.Generators
{
    public class Hills : ChunkGenerator
    {
        public Hills(IGlobeConfig globe, IBlockSet blockSet) : base(globe)
        {
            _lands = new[]
            {
                blockSet[BlockSet.StoneID], blockSet[BlockSet.DirtID], blockSet[BlockSet.DirtWithGrassID],
                blockSet[BlockSet.SnowID], blockSet[BlockSet.SandID], blockSet[BlockSet.IceID],
                blockSet[BlockSet.LavaID]
            };

            _air = blockSet[BlockSet.AirID];
            //_stone = ;
            //_dirt = blockSet[BlockSet.DirtID];
            //_grass = blockSet[BlockSet.DirtWithGrassID];

            var time = Stopwatch.StartNew();
            _planeMesh = CellMeshGenerator.Generate(globe.Bounds.Size.X, globe.Bounds.Size.X, globe.Seed, 3);
            time.Stop();

            Log.Trace("Prepared mesh for plane, time: {0} msec", time.ElapsedMilliseconds);
            
        }

        protected override ChunkContents GenerateSync(Vector2i position)
        {
            var chunkCenterPos = new Vector2(position.X*16 + 8, position.Z*16 + 8);
            var cell = _planeMesh.First(c => c.IsContains(chunkCenterPos));

            Block fillBlock;
            if (cell != null && cell.IsClosed)
                fillBlock = _lands[cell.Id % _lands.Length];
            else
                fillBlock = _air;

            var heightmap = new byte[Chunk.SizeX*Chunk.SizeZ];
            var blocks = new BlockData[Chunk.BlocksCount];
            var result = new ChunkContents(position, blocks, heightmap);

            var chunkBaseX = position.X * Chunk.SizeX;
            var chunkBaseZ = position.Z * Chunk.SizeZ;

            for (var x = 0; x < Chunk.SizeX; x++)
            {
                var worldX = chunkBaseX + x;
                for (int z = 0; z < Chunk.SizeZ; z++)
                {
                    var worldZ = chunkBaseZ + z;
                    var height = (FastIntPerlinNoise.noise(worldX / 160f, worldZ / 160f, 2) / 3/* + FastIntPerlinNoise.noise(worldX / 40f, worldZ / 40f, 1)*/);
                    //var height = (byte)(((SimplexNoise.noise(worldX / 80f, worldZ / 80f) + 1) * 20) + ((SimplexNoise.noise(worldX / 40f, worldZ / 40f) + 1) * 10) + ((SimplexNoise.noise(worldX / 20f, worldZ / 20f) + 1) * 5) + 40);

                    if (height > Chunk.SizeY - 1)
                        height = Chunk.SizeY - 1;

                    heightmap[x * Chunk.SizeX + z] = (byte)height;
                    for (var y = 0; y <= height; y++)
                        result.SetBlockData(x, y, z, new BlockData(fillBlock));
                }
            }

            return result;
        }

        private readonly Block _stone;
        private readonly Block _grass;
        private readonly Block _dirt;
        private Cell[] _planeMesh;
        private Block[] _lands;
        private Block _air;
    }
}