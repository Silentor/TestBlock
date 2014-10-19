using Silentor.TB.Client.Maps;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Common.Tools;

namespace Assets.Code.Benchmarks
{
    public class TestGenerator
    {
        public TestGenerator(IGlobeConfig globe, IBlockSet blockSet)
        {
            _stone = blockSet[BlockSet.StoneID];
            _grass = blockSet[BlockSet.DirtID];
        }

        public ChunkContents GenerateSync(Vector2i position)
        {
            var heightmap = new byte[Chunk.SizeX * Chunk.SizeZ];
            var blocks = new BlockData[Chunk.BlocksCount];
            var result = new ChunkContents(position, blocks, heightmap);

            var chunkBaseX = position.X * Chunk.SizeX;
            var chunkBaseZ = position.Z * Chunk.SizeZ;

            for (int x = 0; x < Chunk.SizeX; x++)
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
                    for (int y = 0; y <= height; y++)
                        result.SetBlockData(x, y, z, new BlockData(_grass));
                }
            }

            return result;
        }

        private readonly Block _stone;
        private readonly Block _grass;
    }
}