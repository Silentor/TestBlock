using Silentor.TB.Client.Maps;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Client.Maps
{
    public interface IChunkFactory
    {
        Chunk Create(ChunkContents content);
    }
}