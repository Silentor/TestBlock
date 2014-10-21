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

        private void OnTimePhysicTick(Time.Timer timer)
        {
            var startTime = timer.TimeMs;
            SimulatePlayers();
            var playersTime = timer.TimeMs;

            Log.Trace("{1} players simulated for {0} ms", playersTime - startTime, _world.Players.Count);
        }

        private void SimulatePlayers()
        {
            //Simulate one step of physic
            if(_world.Players.Count > 16)
                Parallel.ForEach(_world.Players, playerController => playerController.Simulate());
            else
                foreach (var simulator in _world.Players)
                    simulator.Simulate();

            //Entity collision resolve
            //...

            //Update controllers for result of simulation
            if (_world.Players.Count > 16)
                Parallel.ForEach(_world.Players, playerController => playerController.Update());
            else
                foreach (var playerController in _world.Players)
                    playerController.Update();
        }

    }
}
