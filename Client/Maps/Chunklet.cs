using System;
using Silentor.TB.Client.Maps;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using UnityEngine;

namespace Silentor.TB.Client.Maps
{
    public class Chunklet
    {
        public const int SizeXBits = Chunk.SizeXBits;
        public const int SizeYBits = Chunk.SizeYBits - Chunk.ChunkletsCountBits;
        public const int SizeZBits = Chunk.SizeZBits;
        public const int SizeX = 1 << SizeXBits;
        public const int SizeY = 1 << SizeYBits;
        public const int SizeZ = 1 << SizeZBits;
        public const int BlocksCount = SizeX * SizeY * SizeZ;
        public const int MinX = 0;
        public const int MaxX = SizeX - 1;
        public const int MinY = 0;
        public const int MaxY = SizeY - 1;
        public const int MinZ = 0;
        public const int MaxZ = SizeZ - 1;

        /// <summary>
        /// Coordinates inside chunk
        /// </summary>
        public Vector3i Position { get; private set; }

        /// <summary>
        /// Count of nontransparent blocks
        /// </summary>
        public int SolidBlocksCount { get; set; }

        /// <summary>
        /// Count of air blocks
        /// </summary>
        public int EmptyBlocksCount { get; set; }

        public bool IsEmpty
        {
            get { return EmptyBlocksCount == BlocksCount; }
        }

        public Chunk Chunk { get; private set; }

        public Chunklet(Vector3i position, Chunk chunk)
        {
            SolidBlocksCount = 0;
            EmptyBlocksCount = BlocksCount;
            Position = position;
            Chunk = chunk;
        }

        public bool IsSolid
        {
            get { return SolidBlocksCount == BlocksCount; }
        }

        [Obsolete("Slower than Chunk.GetBlock()")]
        public virtual BlockData GetBlock(Vector3i pos)
        {
            return GetBlock(pos.X, pos.Y, pos.Z);
        }

        [Obsolete("Slower than Chunk.GetBlock()")]
        public virtual BlockData GetBlock(int x, int y, int z)
        {
            return Chunk.GetBlock(x, (Position.Y << SizeYBits) + y, z);
        }

        private static readonly Vector3i MaxPositionVector = new Vector3i(SizeX - 1, SizeY - 1, SizeZ - 1);

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
        /// From block to chunklet
        /// </summary>
        /// <param name="blockPosition"></param>
        /// <returns></returns>
        public static Vector3i ToChunkletPosition(Vector3i blockPosition)
        {
            return ToChunkletPosition(blockPosition.X, blockPosition.Y, blockPosition.Z);
        }

        /// <summary>
        /// From block to chunklet
        /// </summary>
        /// <param name="blockPositionX"></param>
        /// <param name="blockPositionY"></param>
        /// <param name="blockPositionZ"></param>
        /// <returns></returns>
        public static Vector3i ToChunkletPosition(int blockPositionX, int blockPositionY, int blockPositionZ)
        {
            var chunkX = blockPositionX >> SizeXBits;
            var chunkY = blockPositionY >> SizeYBits;
            var chunkZ = blockPositionZ >> SizeZBits;
            return new Vector3i(chunkX, chunkY, chunkZ);
        }

        /// <summary>
        /// From world to local position
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static Vector3i ToLocalPosition(Vector3i worldPosition)
        {
            return ToLocalPosition(worldPosition.X, worldPosition.Y, worldPosition.Z);
        }

        /// <summary>
        /// From world to local position
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3i ToLocalPosition(int worldX, int worldY, int worldZ)
        {
            var localX = worldX & (SizeX - 1);
            var localY = worldY & (SizeY - 1);
            var localZ = worldZ & (SizeZ - 1);
            return new Vector3i(localX, localY, localZ);
        }

        /// <summary>
        /// From chunklet and block to world position
        /// </summary>
        /// <param name="chunkletPosition"></param>
        /// <param name="localPosition"></param>
        /// <returns></returns>
        public static Vector3i ToWorldPosition(Vector3i chunkletPosition, Vector3i localPosition)
        {
            var worldX = (chunkletPosition.X << SizeXBits) + localPosition.X;
            var worldY = (chunkletPosition.Y << SizeYBits) + localPosition.Y;
            var worldZ = (chunkletPosition.Z << SizeZBits) + localPosition.Z;
            return new Vector3i(worldX, worldY, worldZ);
        }

        /// <summary>
        /// Bounds of chunklet in world-pos
        /// </summary>
        public static Bounds3i ToChunkletBounds(Vector3i chunkletPosition)
        {
            return new Bounds3i(ToWorldPosition(chunkletPosition, Vector3i.Zero), ToWorldPosition(chunkletPosition, MaxPositionVector));
        }

        public override string ToString()
        {
            return String.Format("Chunklet [{0}, {1}, {2}]", Position.X, Position.Y, Position.Z);
        }
    }
}