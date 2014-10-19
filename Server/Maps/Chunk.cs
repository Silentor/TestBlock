using System;
using System.Diagnostics;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Server.Maps
{
    public class Chunk
    {
        public const int SizeXBits = ChunkContents.SizeXBits;
        public const int SizeYBits = ChunkContents.SizeYBits;
        public const int SizeZBits = ChunkContents.SizeZBits;
        public const int SizeX = ChunkContents.SizeX;
        public const int SizeY = ChunkContents.SizeY;
        public const int SizeZ = ChunkContents.SizeZ;
        public const int BlocksCount = ChunkContents.BlocksCount;

        public const int MaxY = SizeY - 1;

        public const int ChunkletsCountBits = 3;
        public const int ChunkletsCount = 1 << ChunkletsCountBits;

        private readonly BlockData[] _blocks;
        private readonly byte[] _heightMap;

        public Vector2i Position { get; protected set; }

        public int Seed { get; private set; }

        public Random Randomizer { get; private set; }

        public Chunk(int worldSeed, ChunkContents contents)
        {
            Seed = GenerateChunkSeed(worldSeed, contents.Position);

            Position = contents.Position;
            Randomizer = new Random(Seed);

            Debug.Assert(contents.Blocks.Length == BlocksCount, "Blocks count is incorrect");

            _blocks = contents.Blocks;
            _heightMap = contents.HeightMap;
        }

        public void SetBlock(BlockData block, Vector3i pos)
        {
            SetBlock(block, pos.X, pos.Y, pos.Z);
        }

        public void SetBlock(BlockData block, int x, int y, int z)
        {
            _blocks[y + SizeY * x + SizeY * SizeX * z] = block;
        }

        public BlockData GetBlock(Vector3i pos)
        {
            return GetBlock(pos.X, pos.Y, pos.Z);
        }

        public BlockData GetBlock(int x, int y, int z)
        {
            return _blocks[y + SizeY * x + SizeY * SizeX * z];
        }

        public static bool IsCorrectLocalPosition(Vector3i local)
        {
            return IsCorrectLocalPosition(local.X, local.Y, local.Z);
        }

        public static bool IsCorrectLocalPosition(int x, int y, int z)
        {
            return (x & SizeX - 1) == x &&
                   (y & SizeY - 1) == y &&
                   (z & SizeZ - 1) == z;
        }

        /// <summary>
        /// World block position to chunk position
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static Vector2i ToChunkPosition(Vector3i worldPos)
        {
            return ToChunkPosition(worldPos.X, worldPos.Z);
        }

        public static Vector2i ToChunkPosition(Vector2i worldPos)
        {
            return ToChunkPosition(worldPos.X, worldPos.Z);
        }

        public static Vector2i ToChunkPosition(int x, int z)
        {
            return new Vector2i(x >> SizeXBits, z >> SizeZBits);
        }

        /// <summary>
        /// Get block position inside chunk
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static Vector3i ToLocalPosition(Vector3i worldPos)
        {
            return ToLocalPosition(worldPos.X, worldPos.Y, worldPos.Z);
        }

        public static Vector3i ToLocalPosition(int x, int y, int z)
        {
            return new Vector3i(x & (SizeX - 1), y, z & (SizeZ - 1));
        }

        /// <summary>
        /// Convert chunk and block to world
        /// </summary>
        /// <param name="chunkPosition"></param>
        /// <param name="localPosition"></param>
        /// <returns></returns>
        public static Vector3i ToWorldPosition(Vector2i chunkPosition, Vector3i localPosition)
        {
            return ToWorldPosition(chunkPosition, localPosition.X, localPosition.Y, localPosition.Z);
        }

        public static Vector3i ToWorldPosition(Vector2i chunkPosition, int localX, int localY, int localZ)
        {
            var worldX = (chunkPosition.X << SizeXBits) + localX;
            var worldZ = (chunkPosition.Z << SizeZBits) + localZ;
            return new Vector3i(worldX, localY, worldZ);
        }

        internal static int GenerateChunkSeed(int worldSeed, Vector2i pos)
        {
            return worldSeed ^ (512 + pos.X) << 21 ^ (512 + pos.Z) << 11;
        }

        public ChunkContents ToChunkContents()
        {
            return new ChunkContents(Position, _blocks, _heightMap);
        }

        public override string ToString()
        {
            return string.Format("[Chunk: ({0}, {1})]", Position.X, Position.Z);
        }

        public override int GetHashCode()
        {
            return (512 + Position.X) << 11 ^ (512 + Position.Z);
        }
    }
}