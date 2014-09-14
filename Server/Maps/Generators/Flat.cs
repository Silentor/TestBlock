using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Server.Maps.Generators
{
    public class Flat : ChunkGenerator
    {
        public Flat(IGlobeConfig globe, byte height, IBlockSet blockSet) : base(globe)
        {
            _height = height;
            _stone = blockSet[BlockSet.StoneID];
        }

        protected override ChunkContents GenerateSync(Vector2i position)
        {
            var heightmap = new byte[Chunk.SizeX*Chunk.SizeZ];
            var blocks = new BlockData[Chunk.BlocksCount];
            var result = new ChunkContents(position, blocks, heightmap);

            for (byte x = 0; x < Chunk.SizeX; x++)
                for (byte z = 0; z < Chunk.SizeZ; z++)
                {
                    heightmap[x*Chunk.SizeX + z] = _height;
                    for (byte y = 0; y <= _height; y++)
                        result.SetBlockData(x, y, z, new BlockData(_stone));
                }

            return result;
        }

        private readonly byte _height;
        private readonly Block _stone;
    }
}