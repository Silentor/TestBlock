using System;
using System.Linq;
using NLog;
using NLog.Config;
using Silentor.FBRLogger.NLogTarget;
using Silentor.TB.Client.Config;

namespace Silentor.TB.Client.Tools
{
    public class LoggingBase
    {
        private ISystemConfig _config;
        protected static Logger Log;

        public LoggingBase(ISystemConfig config)
        {
            _config = config;

            //Init NLog
            LogManager.ThrowExceptions = true;

            //Avoid xml configuration to prevent System.Xml overhead
            var logConfig = new LoggingConfiguration();

            //var remoteTarget = new NLogViewerTarget { Address = _config.LogViewerAddr };
            var remoteTarget = new FBRTarget { Host = _config.LoggerAddress, Port = _config.LoggerPort };
            logConfig.AddTarget("remote", remoteTarget);
            logConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, remoteTarget));

            LogManager.Configuration = logConfig;

            Log = LogManager.GetLogger("Unity");
            Log.Info("NLog initialized. Targets is " + String.Join(", ", logConfig.ConfiguredNamedTargets.Select(t => t.ToString()).ToArray()));
        }
    }
}