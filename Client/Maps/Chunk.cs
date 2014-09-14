using System;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Client.Maps
{
    /// <summary>
    /// Chunk of blocks
    /// </summary>
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

        public Vector2i Position { get; private set; }

        public IMap Map { get; private set; }

        private void InitChunk(IMap map, Vector2i pos)
        {
            if (map == null) throw new ArgumentNullException("map");

            _chunklets = new Chunklet[ChunkletsCount];
            Position = pos;
            Map = map;
        }

        public Chunk(IMap map, ChunkContents contents)
        {
            InitChunk(map, contents.Position);

            _blocks = contents.Blocks;
            _heightMap = contents.HeightMap;

            for (int chunkletIndex = 0; chunkletIndex < ChunkletsCount; chunkletIndex++)
            {
                var chunklet = new Chunklet(new Vector3i(Position.X, chunkletIndex, Position.Z), this);
                _chunklets[chunkletIndex] = chunklet;
            }
        }

        public Chunklet GetChunklet(int yPosition)
        {
            return _chunklets[yPosition];
        }

        public BlockData GetBlock(Vector3i pos)
        {
            return GetBlock(pos.X, pos.Y, pos.Z);
        }

        public BlockData GetBlock(int x, int y, int z)
        {
#if UNITY_EDITOR || DEBUG
            if(!IsCorrectLocalPosition(x, y, z)) throw new ArgumentOutOfRangeException("x, y, z", string.Format("Block position out of range: {0}, {1}, {2}", x, y, z));
#endif

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
            return new Vector2i(x >> Chunklet.SizeXBits, z >> Chunklet.SizeZBits);
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
            int worldX = (chunkPosition.X << Chunklet.SizeXBits) + localX;
            int worldZ = (chunkPosition.Z << Chunklet.SizeZBits) + localZ;
            return new Vector3i(worldX, localY, worldZ);
        }

        public ChunkContents ToChunkContents()
        {
            return new ChunkContents(Position, _blocks, _heightMap);
        }

        private readonly BlockData[] _blocks;

        //Y position of topmost nonempty block
        private readonly byte[] _heightMap;
        private Chunklet[] _chunklets;

        public override string ToString()
        {
            return string.Format("[Chunk: ({0}, {1})]", Position.X, Position.Z);
        }
    }
}