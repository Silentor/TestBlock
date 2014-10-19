using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NLog;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Server.Maps.Generators
{
    public abstract class ChunkGenerator : IChunkGenerator
    {
        public ChunkGenerator(IGlobeConfig globe)
        {
            Seed = globe.Seed;

            _generator = new ActionBlock<GenerateChunkMessage>(gcm =>
            {
                try
                {
                    var chunk = GenerateSync(gcm.Position);
                    Log.Trace("Generated chunk data {0}", gcm.Position);
                    gcm.Result.SetResult(chunk);
                }
                catch (Exception ex)
                {
                    gcm.Result.SetException(ex);
                }
            });

            _buffer.LinkTo(_generator, new DataflowLinkOptions(){PropagateCompletion = true});

            Log.Trace("Started chunk generator");
        }

        public Task<ChunkContents> Generate(Vector2i position)
        {
            var result = new TaskCompletionSource<ChunkContents>();
            _buffer.Post(new GenerateChunkMessage {Position = position, Result = result});
            return result.Task;
        }

        protected abstract ChunkContents GenerateSync(Vector2i position);

        protected readonly int Seed;

        #region Pipeline
        private readonly ActionBlock<GenerateChunkMessage> _generator;
        private readonly BufferBlock<GenerateChunkMessage> _buffer = new BufferBlock<GenerateChunkMessage>();
        #endregion

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private struct GenerateChunkMessage
        {
            public Vector2i Position;
            public TaskCompletionSource<ChunkContents> Result;
        }


    }
}