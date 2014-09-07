using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Silentor.TB.Client.Config;
using Silentor.TB.Client.Tools;
using Silentor.TB.Client.Tools;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Serialization;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Client.Storage
{
    /// <summary>
    /// Naive chunks cache. Not thread safe. Works asynchronously in main thread, one operation per frame
    /// </summary>
    public class ChunkStorage : IChunkStorage
    {
        public ChunkStorage(IApplicationEvents appEvents, IGameConfig gameConfig)
        {
            _capacity = gameConfig.ChunkCacheSize;
            _appEvents = appEvents;
            _storage = new Dictionary<Vector2i, StoredChunk>(_capacity);
            _serializer = new CommandSerializer();

            _appEvents.FrameTick += AppEventsFrameTickTick;
        }

        public void Store(ChunkContents chunkContents)
        {
            _storeQueue.Enqueue(chunkContents);
        }

        public bool Retrieve(Vector2i position)
        {
            //Check storage
            StoredChunk alreadyStored;
            if (!_storage.TryGetValue(position, out alreadyStored))
            {
                Log.Trace("Storage miss for chunk content at {0}", position);
                return false;
            }

            //Refresh access time
            alreadyStored.AccessTime = _newestAccessTime++;
            _storage[position] = alreadyStored;

            Log.Trace("Storage hit for chunk content at {0}", position);
            _retrieveQueue.Enqueue(alreadyStored);
            return true;
        }

        public event Action<ChunkContents> Retrieved;

        private readonly int _capacity;
        private readonly IApplicationEvents _appEvents;
        private readonly Dictionary<Vector2i, StoredChunk> _storage;
        private readonly CommandSerializer _serializer;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private int _newestAccessTime;
        private readonly Queue<ChunkContents> _storeQueue = new Queue<ChunkContents>();
        private readonly Queue<StoredChunk> _retrieveQueue = new Queue<StoredChunk>();

        private void DoRetrieved(ChunkContents chunk)
        {
            if (Retrieved != null)
                Retrieved(chunk);
        }

        /// <summary>
        /// Store one chunk per frame
        /// </summary>
        private void AppEventsFrameTickTick()
        {
            if (_retrieveQueue.Count > 0)
                RetrieveAsync(_retrieveQueue.Dequeue());

            if (_storeQueue.Count > 0)
                StoreAsync(_storeQueue.Dequeue());
        }

        private void RetrieveAsync(StoredChunk storedChunk)
        {
            var chunkContent = (ChunkContents)_serializer.Decode(storedChunk.PackedChunk, true);
            Log.Trace("Retrieved chunk content {0}", chunkContent.Position);

            DoRetrieved(chunkContent);
        }

        private void StoreAsync(ChunkContents chunkContents)
        {
            //Prepare data
            int length;
            var data = _serializer.Encode(chunkContents, out length, true);
            if (data.Length != length)
            {
                var trimmed = new byte[length];
                Array.Copy(data, trimmed, length);
                data = trimmed;
            }

            //Check if same chunk already stored
            _storage.Remove(chunkContents.Position);

            //Free some space if needed
            if (_storage.Count == _capacity)
            {
                //Find oldest element
                var oldestAccessTime = int.MaxValue;
                var oldestKvp = _storage.First();
                foreach (var kvp in _storage)
                {
                    if (kvp.Value.AccessTime < oldestAccessTime)
                    {
                        oldestKvp = kvp;
                        oldestAccessTime = kvp.Value.AccessTime;
                    }
                }

                _storage.Remove(oldestKvp.Key);
            }

            var stored = new StoredChunk(_newestAccessTime++, data);
            _storage.Add(chunkContents.Position, stored);

            Log.Trace("Stored chunk content {0}", chunkContents.Position);
        }

        private struct StoredChunk
        {
            public readonly byte[] PackedChunk;
            public int AccessTime;

            public StoredChunk(int accessTime, byte[] packedChunk)
            {
                AccessTime = accessTime;
                PackedChunk = packedChunk;
            }
        }
    }
}
