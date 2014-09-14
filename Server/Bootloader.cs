using System;
using System.Linq;
using NLog;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Blocks;
using Silentor.TB.Common.Network;
using Silentor.TB.Server.Config;
using Silentor.TB.Server.Maps;
using Silentor.TB.Server.Simulation;
using Silentor.TB.Server.Time;
using Silentor.TB.Server.Tools;

namespace Silentor.TB.Server
{
    /// <summary>
    /// Primary configuration and run server. Agnostic of project type
    /// </summary>
    public class Bootloader
    {
        public bool IsPaused { get { return _timer.IsPaused; } }

        public Bootloader(Config config)
        {
            var configuredTargets = LogManager.Configuration.ConfiguredNamedTargets.Select(t => t.ToString());
            Log.Info("NLog initialized. Targets is " + String.Join(", ", configuredTargets.ToArray()));

            _config = config;

            IGlobeConfig globe = new GlobeConfig();
            var blockSet = new BlockSet();

            _globe = new Globe(globe, blockSet);
            _timer = new Timer();
            _server = new Network.Server(_config.Port);
            _world = new World(_server, _globe, _timer);
            _gameLoop = new GameLoop(_world, _timer);
            _timer = _gameLoop.Timer;
            _statistics = new Statistics(_server);

            _engine = new Engine(_server, _world, _timer);
        }

        public void Start()
        {
            _gameLoop.Start();
        }

        public void Stop()
        {
            _timer.Terminate();
        }

        public void Pause()
        {
            _timer.IsPaused = true;
        }

        public void Resume()
        {
            _timer.IsPaused = false;
        }

        private readonly Config _config;
        private static Logger Log = LogManager.GetCurrentClassLogger();
        private Silentor.TB.Server.Network.Server _server;
        private readonly GameLoop _gameLoop;
        private readonly Timer _timer;
        private World _world;
        private Globe _globe;
        private Engine _engine;
        private Statistics _statistics;


        public class Config
        {
            public string LoggerAddr = "192.168.0.100";
            public int LoggerPort = 9998;
            public int SecurityPort = Settings.SecurityPort;
            public int Port = Settings.Port;
        }



    }
}
