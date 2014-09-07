using Silentor.TB.Client.Tools;

namespace Silentor.TB.Client.Config
{
    public interface ISystemConfig
    {
        string LoggerAddress { get; }

        int LoggerPort { get; }

        string ServerAddress { get; }

        int ServerPort { get; }

        IApplicationEvents ApplicationEvents { get; }
    }
}
