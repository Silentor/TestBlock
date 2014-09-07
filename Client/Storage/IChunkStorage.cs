using System;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Client.Storage
{
    /// <summary>
    /// Cache storage for chunks, to decrease pressure on server
    /// </summary>
    public interface IChunkStorage
    {
        /// <summary>
        /// Put chunk to cache
        /// </summary>
        /// <param name="chunkContents"></param>
        void Store(ChunkContents chunkContents);

        /// <summary>
        /// Async retrieve chunk from cache. Chunk retrieved in <see cref="Retrieved"/>
        /// </summary>
        /// <param name="position">Position of retrieved chunk</param>
        /// <returns>true if chunk present in cache</returns>
        bool Retrieve(Vector2i position);

        /// <summary>
        /// Fired when retrieved chunk is ready
        /// </summary>
        event Action<ChunkContents> Retrieved;
    }
}