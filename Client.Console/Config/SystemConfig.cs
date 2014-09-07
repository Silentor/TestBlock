using Silentor.TB.Client.Config;
using Silentor.TB.Client.Tools;

namespace Silentor.TB.Client.Console
{
    internal class SystemConfig : ISystemConfig
    {
        public string LoggerAddress { get; private set; }

        public int LoggerPort { get; private set; }

        public string ServerAddress { get; private set; }

        public int ServerPort { get; private set; }

        public IApplicationEvents ApplicationEvents { get; private set; }

        public SystemConfig(IApplicationEvents applicationEvents, int serverPort, string serverAddress, int loggerPort, string loggerAddress)
        {
            ApplicationEvents = applicationEvents;
            ServerPort = serverPort;
            ServerAddress = serverAddress;
            LoggerPort = loggerPort;
            LoggerAddress = loggerAddress;
        }
    }
}