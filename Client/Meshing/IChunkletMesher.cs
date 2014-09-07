using Silentor.TB.Client.Maps;

namespace Silentor.TB.Client.Meshing
{
    public interface IChunkletMesher
    {
        MeshData Render(Chunklet chunklet, Chunklet[] neighbours);
    }
}