using System;
using System.Collections.Generic;
using Silentor.TB.Client.Maps;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Common.Maps
{
    /// <summary>
    /// Base map properties
    /// </summary>
    public interface IMapConfig
    {
        /// <summary>
        /// Coverage area of the map
        /// </summary>
        Bounds2i Bounds { get; }
    }

    /// <summary>
    /// View map and player level access
    /// </summary>
    public interface IMap : IMapConfig, IEnumerable<Chunk>
    {
        int Seed { get; }

        bool IsCorrectChunkPosition(Vector2i position);

        Chunk GetChunk(Vector2i chunkPos);

        BlockData GetBlockData(Vector3i pos);

        BlockData GetBlockData(int x, int y, int z);

        bool IsChunkPresent(Vector2i position);

        void Resize(Vector2i offset, ICollection<Chunk> removedChunks, ICollection<Vector2i> addedChunkPlaces);

        Chunklet GetChunklet(Vector3i chunkletPos);

        bool IsCorrectChunkletPosition(Vector3i position);

        event Action<Chunk> ChunkAdded;

        event Action<Chunk> ChunkRemoved;
    }

    /// <summary>
    /// World level edit map
    /// </summary>
    public interface IMapEditor : IMap
    {
        void SetChunk(Chunk chunk);
    }

    /// <summary>
    /// Code-style map config
    /// </summary>
    public class MapConfig : IMapConfig
    {
        public Bounds2i Bounds { get; private set; }

        public MapConfig(Bounds2i bounds)
        {
            Bounds = bounds;
        }
    }
}
