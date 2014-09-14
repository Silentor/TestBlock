using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Server.Maps.Generators
{
    public class Shuffle : ChunkGenerator
    {
        public Shuffle(IGlobeConfig globe, IBlockSet blockSet) : base(globe)
        {
            _blockSet = blockSet;
        }

        protected override ChunkContents GenerateSync(Vector2i position)
        {
            var rnd = new System.Random(Chunk.GenerateChunkSeed(Seed, position));

            var blocks = new BlockData[Chunk.BlocksCount];
            for (int i = 0; i < blocks.Length; i++)
            {
                var value = rnd.Next(BlockSet.OpaqueStartIndex, BlockSet.BedrockID);
                blocks[i] = new BlockData(_blockSet[value]);
            }

            return new ChunkContents(position, blocks, new byte[256]);
        }

        private readonly IBlockSet _blockSet;
    }
}
