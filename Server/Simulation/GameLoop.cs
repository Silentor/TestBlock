using System.Threading.Tasks;
using NLog;

namespace Silentor.TB.Server.Simulation
{
    /// <summary>
    /// Process simulation loops
    /// </summary>
    public class GameLoop
    {
        private readonly World _world;

        public Time.Timer Timer { get; private set; }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public GameLoop(World world, Time.Timer timer)
        {
            _world = world;
            Timer = timer;

            Timer.PhysicTick += OnTimePhysicTick;
        }

        public void Start()
        {
            Timer.Start();
        }

        private void OnTimePhysicTick(Time.Timer timer)
        {
            var startTime = timer.TimeMs;
            SimulatePlayers();
            var playersTime = timer.TimeMs;

            Log.Trace("{1} players simulated for {0} ms", playersTime - startTime, _world.Players.Count);
        }

        private void SimulatePlayers()
        {
            if(_world.Players.Count > 16)
                Parallel.ForEach(_world.Players, sim => sim.SimulatePlayer());
            else
                foreach (var simulator in _world.Players)
                    simulator.SimulatePlayer();

            //Entity collision resolve
            //...

            if (_world.Players.Count > 16)
                Parallel.ForEach(_world.Players, sim => sim.Controller.UpdateClient());
            else
                foreach (var simulator in _world.Players)
                    simulator.Controller.UpdateClient();
        }

    }
}
