using System;
using Lidgren.Network;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Common.Network.Serialization;

namespace Silentor.TB.Common.Maps.Chunks
{
    public class ChunkContents : IChunkContent
    {
        public const int SizeXBits = 4;
        public const int SizeYBits = 7;
        public const int SizeZBits = 4;
        public const int SizeX = 1 << SizeXBits;
        public const int SizeY = 1 << SizeYBits;
        public const int SizeZ = 1 << SizeZBits;
        public const int BlocksCount = SizeX * SizeY * SizeZ;

        public Vector2i Position { get; private set; }

        public byte[] HeightMap { get; private set; }

        public BlockData[] Blocks { get; private set; }

        public ChunkContents(Vector2i position, BlockData[] blocks, byte[] heightmap)
        {
            if (blocks == null) throw new ArgumentNullException("blocks");
            if (heightmap == null) throw new ArgumentNullException("heightmap");
            if (blocks.Length != ChunkContents.BlocksCount)
                throw new ArgumentOutOfRangeException("blocks", blocks.Length, "Blocks count incorrect");
            if (heightmap.Length != ChunkContents.SizeX * ChunkContents.SizeZ)
                throw new ArgumentOutOfRangeException("heightmap", heightmap.Length, "Heightmap count incorrect");

            Blocks = blocks;
            HeightMap = heightmap;
            Position = position;
        }

        public BlockData GetBlockData(int x, int y, int z)
        {
            return Blocks[y + SizeY * x + SizeY * SizeX * z];
        }

        //todo consider rewrite to Block[ConvertPosition(x, y, z)] = blockData
        public void SetBlockData(int x, int y, int z, BlockData block)
        {
            Blocks[y + SizeY * x + SizeY * SizeX * z] = block;
        }

        public override string ToString()
        {
            return string.Format("[ChunkContents: ({0}, {1})]", Position.X, Position.Z);
        }
    }

    public interface IChunkContent
    {
        Vector2i Position { get; }

        BlockData[] Blocks { get; }

        byte[] HeightMap { get; }
    }
}
