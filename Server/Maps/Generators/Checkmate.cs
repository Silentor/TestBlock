using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Server.Maps.Generators
{
    public class Checkmate : ChunkGenerator
    {
        public Checkmate(IGlobeConfig globe, IBlockSet blockSet) : base(globe)
        {
            _stone = blockSet[BlockSet.StoneID];
        }

        protected override ChunkContents GenerateSync(Vector2i position)
        {
            var heightmap = new byte[Chunk.SizeX*Chunk.SizeZ];
            var blocks = new BlockData[Chunk.BlocksCount];

            for (int i = 0; i < blocks.Length; i++)
            {
                if (i%3 != 0)
                    blocks[i] = new BlockData(_stone);
                else
                    blocks[i] = new BlockData(null);
            }

            return new ChunkContents(position, blocks, heightmap);
        }

        private readonly Block _stone;
    }
}