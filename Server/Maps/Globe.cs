using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NLog;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Server.Maps.Generators;
using Silentor.TB.Server.Players;

namespace Silentor.TB.Server.Maps
{
    /// <summary>
    /// Main storage for all used chunk. <see cref="Map"/> classes gets Chunks from Globe.
    /// Retreives chunk from himself, generator or cache, discards unused chunks to cache
    /// Based on dictionary for now, on quadtree for future
    /// </summary>
    public class Globe
    {
        public readonly Bounds2i Bounds;

        /// <summary>
        /// All players of the world
        /// </summary>
        public IReadOnlyList<HeroController> Players
        {
            get
            {
                if (_playersChanged)
                    lock (_players)
                        if (_playersChanged)
                            _playersCache = _players.ToArray();

                return _playersCache;
            }
        }

        public Globe(IGlobeConfig globe, IBlockSet blockSet)
        {
            Bounds = globe.Bounds;
            _generator = new Hills(globe, blockSet);
            //_sizeX = Bounds.Size.X;
            _chunks = new ConcurrentDictionary<Vector2i, Chunk>();  //todo should be some spatial optimized data structure
        }

        /// <summary>
        /// Get chunk if present or delayed retrieve it from generator or cache
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task<Chunk> GetChunk(Vector2i position)
        {
            if (!Bounds.Contains(position)) return null;

            //Check globe cache
            Chunk result;
            if(_chunks.TryGetValue(position, out result))
                return result;

            //Chunk is not in globe - generate it and place to globe
            var newChunkData = await _generator.Generate(position);
            var newChunk = new Chunk(666, newChunkData);
            _chunks[position] = newChunk;
            return newChunk;
        }

        public void AddPlayer(HeroController player)
        {
            lock (_players)
            {
                Debug.Assert(!_players.Contains(player));

                _players.Add(player);
                _playersChanged = true;
            }

            Log.Debug("Added player {0} to gameworld", player.Player.Name);
        }

        public void RemovePlayer(HeroController player)
        {
            lock (_players)
            {
                Debug.Assert(_players.Contains(player));

                _players.Remove(player);
                _playersChanged = true;
            }

            Log.Debug("Removed player {0} from gameworld", player.Player.Name);
        }

        private readonly ConcurrentDictionary<Vector2i, Chunk> _chunks;

        private readonly IChunkGenerator _generator;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly List<HeroController> _players = new List<HeroController>();
        private bool _playersChanged = true;
        private IReadOnlyList<HeroController> _playersCache;
    }
}
