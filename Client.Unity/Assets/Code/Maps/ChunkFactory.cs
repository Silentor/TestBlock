using ModestTree.Zenject;
using Silentor.TB.Client.Maps;
using Silentor.TB.Common.Maps;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Client.Maps
{
    public class ChunkFactory : IChunkFactory
    {
        private readonly DiContainer _factory;

        public ChunkFactory(DiContainer factory)
        {
            _factory = factory;
        }

        public Chunk Create(ChunkContents content)
        {
            //_binder.Bind<IChunkContent>().ToValue(content);
            //var newChunk = _binder.GetInstance<IChunkContent>();
            //_binder.Unbind<>();
            return new Chunk(_factory.Resolve<IMapEditor>(), content);
        }
    }
}
