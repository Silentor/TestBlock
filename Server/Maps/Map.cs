using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Server.Maps
{
    /// <summary>
    /// Квадратный массив чанков, где существует игрок (наблюдатель) и симулируемое окружение
    /// Является sliding window относительно Globe. Заботится, чтобы все чанки были заполнены данными
    /// </summary>
    public class Map
    {
        public Bounds2i Bounds { get { return _chunks.Bounds; } }

        public Globe Globe { get; private set; }
        private readonly Grid<Chunk> _chunks;
        private readonly ReaderWriterLockSlim _chunksLock = new ReaderWriterLockSlim();
        private readonly Logger Log;

        public Map(int id, Globe globe, Bounds2i bounds)
        {
            Log = LogManager.GetLogger("Wob.Server.Maps.Map" + id);

            Globe = globe;

            _chunks = new Grid<Chunk>(bounds);

            Log.Debug("Created map, bounds {0}, start filling...", Bounds);

            var fillMapTask = FillMap();
            fillMapTask.Wait();

            Log.Debug("Initial map filling completed");
        }

        public void Slide(Vector2i offset)
        {
            //_chunksLock.EnterWriteLock();

            //Get old chunks
            //foreach (var oldPos in _chunks.Bounds.Substract(newBounds))
            //{
                //var oldChunk = GetChunk(oldPos);
                //result.Old.Add(oldChunk);
                //DoChunkRemoved(oldChunk);
            //}

            //Get new positions
            //foreach (var newPos in newBounds.Substract(_chunks.Bounds))
                //result.New.Add(newPos);

            //Translate grid
            _chunks.Slide(offset, true);

            //_chunksLock.ExitWriteLock();

            Log.Trace("Map slided on {0}, new bounds {1}", offset, Bounds);

            //Fill empty spaces
            var fillMapTask = FillMap();
            //fillMapTask.Wait();

            //return result;
        }

        public Chunk GetChunk(Vector2i position)
        {
            Chunk result = null;

            _chunksLock.EnterReadLock();

            if (_chunks.IsCorrectIndex(position))
                result = _chunks.Get(position);

            _chunksLock.ExitReadLock();

            return result;
        }

        public async Task<Chunk> GetChunkAsync(Vector2i position)
        {
            if (_chunks.IsCorrectIndex(position))
            {
                var chunk = _chunks.Get(position);
                if (chunk != null)
                    return chunk;
                else
                {
                    chunk = await Globe.GetChunk(position);
                    _chunks.Set(chunk, position);

                    Log.Trace("Chunk {0} placed in map", chunk);
                    return chunk;
                }
            }
            else return null;
        }

        public event Action<Chunk> ChunkAdded;

        protected virtual void DoChunkAdded(Chunk chunk)
        {
            var handler = ChunkAdded;
            if (handler != null) handler(chunk);
        }

        /// <summary>
        /// Fill all empty chunks in map
        /// </summary>
        private async Task FillMap()
        {
            //_chunksLock.EnterUpgradeableReadLock();

            //todo consider cancel filling if map is changed
            for (var x = _chunks.MinX; x <= _chunks.MaxX; x++)
                for (var z = _chunks.MinZ; z <= _chunks.MaxZ; z++)
                    if (_chunks.Get(x, z) == null)
                    {
                        var chunk = await GetChunkAsync(new Vector2i(x, z));
                    }

            //_chunksLock.ExitUpgradeableReadLock();
        }

        public BlockData GetBlock(Vector3i worldPosition)
        {
            var chunkPosition = Chunk.ToChunkPosition(worldPosition);
            if (_chunks.IsCorrectIndex(chunkPosition) && worldPosition.Y >= 0 && worldPosition.Y <= Chunk.MaxY)
            {
                var chunk = _chunks.Get(chunkPosition);
                return chunk.GetBlock(Chunk.ToLocalPosition(worldPosition));
            }
            else
                return BlockData.Null;
        }
    }
}
