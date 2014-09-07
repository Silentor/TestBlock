using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Wob.Server.Tools
{
    public class Statistics
    {
        public Statistics(Network.Server server)
        {
            _statisticCollector = Task.Run(async () =>
            {
                while (true)
                {
                    var stats = server.GetStatistic();
                    Log.Trace("Incoming: {0}, decoded: {1}, fails: {2}, encoding: {3}, outgoing: {4}",
                        stats.IncomingCount, stats.DecodedCount, stats.ErrorCount, stats.EncodingCount,
                        stats.OutgoingCount);

                    await Task.Delay(1000);
                }
            });
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private Task _statisticCollector;
    }
}
