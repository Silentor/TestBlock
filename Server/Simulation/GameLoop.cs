using System.Linq;
using System.Threading.Tasks;
using NLog;
using Silentor.TB.Server.Time;

namespace Silentor.TB.Server.Simulation
{
    /// <summary>
    /// Process simulation loops
    /// </summary>
    public class GameLoop
    {
        public GameLoop(World world, Time.Timer timer)
        {
            _world = world;
            _timer = timer;

            _timer.PhysicTick += OnTimePhysicTick;
        }

        public void Start()
        {
            _timer.Start();
        }

        private readonly World _world;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Timer _timer;

        private void OnTimePhysicTick(Timer timer)
        {
            var startTime = timer.TimeMs;
            var simulatedCount = SimulatePlayers();
            var playersTime = timer.TimeMs;

            Log.Trace("{1} players simulated for {0} ms", playersTime - startTime, simulatedCount);
        }

        private int SimulatePlayers()
        {
            var players = _world.Globes.First().Players;

            //Simulate one step of physic
            if (players.Count > 16)
                Parallel.ForEach(players, playerController => playerController.Simulate());
            else
                foreach (var simulator in players)
                    simulator.Simulate();

            //Entity collision resolve
            //...

            //Update controllers for result of simulation
            if (players.Count > 16)
                Parallel.ForEach(players, playerController => playerController.Update());
            else
                foreach (var playerController in players)
                    playerController.Update();

            return players.Count;
        }

    }
}
