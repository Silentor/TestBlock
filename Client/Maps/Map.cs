using System;
using System.Collections;
using System.Collections.Generic;
using NLog;
using Silentor.TB.Common.Exceptions;
using Silentor.TB.Common.Maps;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Client.Maps
{
    /// <summary>
    /// Sliding square array of chunks, provide methods to add\remove chunks
    /// </summary>
    public class Map : IMapEditor
    {
        private readonly IBlockSet _blockSet;

        public Bounds2i Bounds {get { return _chunks.Bounds; }}

        public int Seed { get; private set; }

        /// <summary>
        /// Inclusive map limits or Bounds.Inifinity for unlimite map
        /// </summary>
        public Bounds2i Limits { get; private set; }


        public Map(IMapConfig mapConfig, IBlockSet blockSet)
        {
            _blockSet = blockSet;
            _chunks = new Grid<Chunk>(mapConfig.Bounds);
            Limits = Bounds2i.Infinite;

            Log.Trace("Created Map, bounds: {0}", mapConfig.Bounds);
        }

        public BlockData  GetBlockData(Vector3i pos)
        {
            return GetBlockData(pos.X, pos.Y, pos.Z);
        }

        /// <summary>
        /// Get BlockData by world position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public BlockData GetBlockData(int x, int y, int z)
        {
            var chunkPos = Chunk.ToChunkPosition(x, z);
            if (!IsCorrectChunkPosition(chunkPos))
                throw new ArgumentOutOfRangeException("position", new Vector3i(x, y, z), "Chunk position " + chunkPos + " for given Block position is incorrect");
            var chunk = GetChunk(chunkPos);
            if(chunk == null)
                throw new ChunkException("Chunk " + chunkPos + " is null");
            return chunk.GetBlock(Chunk.ToLocalPosition(x, y, z));
        }

        public Block GetBlock(Vector3i pos)
        {
            return _blockSet[GetBlockData(pos).Id];
        }

        public Block GetBlock(int x, int y, int z)
        {
            return _blockSet[GetBlockData(x, y, z).Id];
        }

        public Chunklet GetChunklet(Vector3i chunkletPos)
        {
            if (chunkletPos.Y < 0 || chunkletPos.Y >= Chunk.ChunkletsCount)
                return null;

            if (!IsCorrectChunkPosition((Vector2i) chunkletPos) || _chunks.Get(chunkletPos.X, chunkletPos.Z) == null)
                return null;

            return _chunks.Get(chunkletPos.X, chunkletPos.Z).GetChunklet(chunkletPos.Y);
        }

        public Chunk GetChunk(Vector2i chunkPos)
        {
            if (!_chunks.IsCorrectIndex(chunkPos))
                return null;

            var chunk = _chunks.Get(chunkPos.X, chunkPos.Z);
            return chunk;
        }

        public void SetChunk(Chunk chunk)
        {
            if (IsCorrectChunkPosition(chunk.Position))
            {
#if UNITY_EDITOR || DEBUG
                if(IsChunkPresent(chunk.Position))
                    throw new ArgumentException("Chunk" + chunk.Position + " already in map!");
#endif
                _chunks.Set(chunk, chunk.Position);
                DoChunkAdded(chunk);

                if(Log.IsTraceEnabled) Log.Trace("Chunk {0} was set to Map", chunk);
            }
        }

        public bool IsCorrectChunkPosition(Vector2i position)
        {
            return _chunks.IsCorrectIndex(position) && Limits.Contains(position);
        }

        public bool IsCorrectChunkletPosition(Vector3i position)
        {
            return IsCorrectChunkPosition((Vector2i)position) && position.Y >= 0 && position.Y < Chunk.ChunkletsCount;
        }

        public bool IsChunkPresent(Vector2i position)
        {
            if (!_chunks.IsCorrectIndex(position)) throw new ArgumentOutOfRangeException("position");
            return _chunks.Get(position) != null;
        }

        public void Resize(Vector2i offset, ICollection<Chunk> removedChunks, ICollection<Vector2i> addedChunkPlaces)
        {
            if(removedChunks == null) removedChunks = new List<Chunk>();
            if(addedChunkPlaces == null) addedChunkPlaces = new List<Vector2i>();
            var newBounds = _chunks.Bounds.Translate(offset);

            //Get old chunks
            foreach (var oldPos in _chunks.Bounds.Substract(newBounds))
            {
                var oldChunk = GetChunk(oldPos);
                removedChunks.Add(oldChunk);
                DoChunkRemoved(oldChunk);
            }

            //Get new positions
            foreach (var newPos in newBounds.Substract(_chunks.Bounds))
                addedChunkPlaces.Add(newPos);

            //Translate grid
            _chunks.Slide(offset, true);
        }

        public event Action<Chunk> ChunkAdded;

        public event Action<Chunk> ChunkRemoved;

        public IEnumerator<Chunk> GetEnumerator()
        {
            foreach (var chunk in _chunks)
                if (chunk != null) yield return chunk;
        }

        private readonly Grid<Chunk> _chunks;

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void DoChunkAdded(Chunk chunk)
        {
            if (ChunkAdded != null)
                ChunkAdded(chunk);
        }

        private void DoChunkRemoved(Chunk chunk)
        {
            if (ChunkRemoved != null)
                ChunkRemoved(chunk);
        }
    }
}
