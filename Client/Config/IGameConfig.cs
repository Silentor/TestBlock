namespace Silentor.TB.Client.Config
{
    public interface IGameConfig
    {
        /// <summary>
        /// Chunk view radius
        /// </summary>
        int ChunkViewRadius { get; }

        int ChunkCacheSize { get; }
    }

}
