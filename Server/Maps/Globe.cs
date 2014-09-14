using System;
using System.Collections.Generic;
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
    /// Based on flat array for now, on quadtree for future
    /// </summary>
    public class Globe
    {
        public readonly Bounds2i Bounds;

        public event Action<Chunk> ChunkAdded;

        public Globe(IGlobeConfig globe, IBlockSet blockSet)
        {
            Bounds = globe.Bounds;
            _generator = new Hills(globe, blockSet);
            _sizeX = Bounds.Size.X;
            _chunks = new Chunk[Bounds.Size.X * Bounds.Size.Z];  //todo should be some spatial optimized data structure
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
            var result = _chunks[Position2Index(position.X, position.Z)];
            if (result != null)
                return result;

            //Chunk is not in globe - generate it and place to globe
            var newChunkData = await _generator.Generate(position);
            var newChunk = new Chunk(666, newChunkData);
            _chunks[Position2Index(position.X, position.Z)] = newChunk;
            return newChunk;
        }

        private int Position2Index(int x, int z)
        {
            return x*Bounds.Size.X + z;
        }

        private void DoChunkAdded(Chunk chunk)
        {
            var handler = ChunkAdded;
            if (handler != null)
                handler(chunk);
        }

        //private readonly Dictionary<Vector2i, ChunkContents> _cache = new Dictionary<Vector2i, ChunkContents>();

        //Currently used chunks
        private readonly Chunk[] _chunks;

        private readonly IChunkGenerator _generator;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public readonly List<Player> Entities = new List<Player>();
        private int _sizeX;
    }
}
