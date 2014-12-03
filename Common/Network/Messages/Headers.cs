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
        ChunkMessage,
        StreamHeader,

        //Players
        EntityUpdate,
        HeroMovement,
        
        Test = 120
    }
}
