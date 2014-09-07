using System.Threading.Tasks;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Wob.Server.Maps.Generators
{
    public interface IChunkGenerator
    {
        Task<ChunkContents> Generate(Vector2i position);
    }
}
