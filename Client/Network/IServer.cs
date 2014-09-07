namespace Silentor.TB.Client.Network
{
    /// <summary>
    /// Server
    /// </summary>
    public interface IServer
    {
        IServerClient ClientConnection { get; }

        IClientServer ServerConnection { get; }
    }
}
