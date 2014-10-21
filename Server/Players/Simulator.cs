using System;
using NLog;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using Silentor.TB.Server.Maps;
using Silentor.TB.Server.Network;
using Silentor.TB.Server.Tools;

namespace Silentor.TB.Server.Players
{
    /// <summary>
    /// Combines player, map and all its environment, responsible for Player - Hero relations
    /// </summary>
    public class Simulator
    {
        public const int SimulationWindowRadius = 6;

        public Map Map { get; private set; }

        public HeroController Controller { get; private set; }

        public Player Player { get; private set; }

        public readonly Client Client;

        public Simulator(Player player, Map map, HeroController controller, Client client)
        {
            Map = map;
            Controller = controller;
            Player = player;
            Client = client;

            

            Log = LogManager.GetLogger("Wob.Server.Players.Simulator" + player.Id);
            Log.Info("Created player simulator {0}", player.Id);
        }

        

        

        private readonly Logger Log;

        public override string ToString()
        {
            return String.Format("Simulator for {0} from {1}", Player, Client);
        }
    }
}
