using System.Threading.Tasks;
using Silentor.TB.Common.Maps.Chunks;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Server.Maps.Generators
{
    public interface IChunkGenerator
    {
        Task<ChunkContents> Generate(Vector2i position);
    }
}
