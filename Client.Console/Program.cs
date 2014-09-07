using System.Linq;
using System.Threading;
using NLog;
using NLog.Config;
using NLog.Targets;
using Silentor.TB.Client.Tools;

namespace Silentor.TB.Client.Console
{
    class Program
    {
        private const int ClientsCount = 1;

        static void Main(string[] args)
        {
            var appEvents = new ApplicationEvents();
            var sysConfig = new SystemConfig(appEvents, 10000, "192.168.0.100", 9998, "192.168.0.100");

            var logging = new LoggingBase(sysConfig);
            InitLogging();
            LogManager.GlobalThreshold = LogLevel.Warn;

            //Create many bots
            foreach (var clientName in Enumerable.Range(0, ClientsCount).Select(id => id.ToString()))
            {
                var client = new BotClient(sysConfig, appEvents, clientName);
                Thread.Sleep(500);

                Log.Warn("Created client {0}", clientName);
            }

            while (true)
            {
                if (System.Console.KeyAvailable)
                {
                    var cmd = System.Console.ReadLine();
                
                    if (cmd == "c")
                    {
                        return;
                    }
                }

                Thread.Sleep(500);
            }
        }

        private static void InitLogging()
        {
            var logConfig = LogManager.Configuration;
            var consoleTarget = new ColoredConsoleTarget()
            {
                Layout = "${time} ${level:uppercase=true} ${logger} ${message}",
                UseDefaultRowHighlightingRules = true
            };
            logConfig.AddTarget("console", consoleTarget);
            logConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, consoleTarget));
            LogManager.Configuration = logConfig;
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    }
}
