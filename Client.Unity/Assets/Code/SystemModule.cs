using System;
using ModestTree.Zenject;
using Silentor.TB.Client.Config;
using Silentor.TB.Client.Debug;
using Silentor.TB.Client.Network;
using Silentor.TB.Client.Tools;

namespace Silentor.TB.Client
{
    public class SystemModule : Installer
    {
        [Inject]
        private readonly Config Settings;

        [Serializable]
        public class Config : ISystemConfig
        {
            public string LoggerAddress = "192.168.0.101";
            public int LoggerPort = 9998;
            public string ServerAddress = "192.168.0.100";
            public int ServerPort = 10000;
            public ApplicationEvents ApplicationEvents;

            string ISystemConfig.LoggerAddress { get { return LoggerAddress; } }
            int ISystemConfig.LoggerPort { get { return LoggerPort; }}
            string ISystemConfig.ServerAddress { get { return ServerAddress; } }
            int ISystemConfig.ServerPort { get { return ServerPort; } }
            IApplicationEvents ISystemConfig.ApplicationEvents { get { return ApplicationEvents; }}
        }


        public override void InstallBindings()
        {
            Container.Bind<IInitializable>().ToSingle<GameRoot>();

            Container.Bind<ISystemConfig>().To(Settings);

            Container.Bind<Logging>().ToSingle();

            Container.Bind<IApplicationEvents>().To(Settings.ApplicationEvents);

            Container.Bind<IServer>().ToSingle<RemoteServer>();
        }
        
    }
}
