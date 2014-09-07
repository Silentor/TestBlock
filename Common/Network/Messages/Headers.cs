namespace Silentor.TB.Common.Network.Messages
{
    public enum Headers : byte
    {
        //Login
        Login,
        LoginResponce,
        Disconnect,
        HeroManagenent,

        //Chunks
        GetChunk,
        ChunkResponce,
        StreamHeader,

        //Players
        EntityUpdate,
        PlayerMovement,
        
    }
}
