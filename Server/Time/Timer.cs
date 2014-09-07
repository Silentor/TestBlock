using System;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace Wob.Server.Time
{
    public class Timer
    {
        /// <summary>
        /// Game ticks from server start
        /// </summary>
        public int TickCount { get; private set; }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                _isPaused = value;
                Log.Debug("Timer run {0}, tick {1}", !value, TickCount);
            }
        }

        /// <summary>
        /// Realtime msec from server start
        /// </summary>
        public int TimeMs { get { return (int)_timer.ElapsedMilliseconds; } }

        public const float DeltaTime = Timestep / 1000f;
        private const int Timestep = 100;

        /// <summary>
        ///Ticks for physics simulations
        /// </summary>
        public event Action<Timer> PhysicTick;

        public Timer()
        {
            IsPaused = true;

            _timerThread = new Thread(TimerWorker);
            _timerThread.IsBackground = true;           
            _timerThread.Name = "TimerWorker";
            _timerThread.Start();
        }

        public void Start()
        {
            Log.Info("Timer started");
            IsPaused = false;
        }

        public void Terminate()
        {
            _isTerminated = true;
            Log.Debug("Timer terminating");
        }

        private void TimerWorker()
        {
            var nextTick = _timer.ElapsedMilliseconds;

            while (!_isTerminated)
            {
                var before = _timer.ElapsedMilliseconds;

                if (!IsPaused)
                {
                    //Log.Trace("Tick started at {0} msec", _timer.ElapsedMilliseconds);

                    try
                    {
                        if (PhysicTick != null)
                            PhysicTick(this);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Exception on PhysicTick", ex);
                    }

                    //Log.Trace("Tick ended at {0} msec", _timer.ElapsedMilliseconds);

                    TickCount++;
                }

                nextTick += Timestep;
                var after = _timer.ElapsedMilliseconds;

                if(after - before > Timestep)
                    Log.Warn("Tick {0} was timeouted, elapsed {1}", TickCount, after - before);

                var sleepTime = nextTick - after;
                if (sleepTime >= 0)
                {
                    Thread.Sleep((int)sleepTime);
                }
                else
                    Log.Warn("Tick {0} lagged, there is no time to sleep, time ahead {1}", TickCount, -sleepTime);
            }

            Log.Info("Timer is terminated, last tick {0}", TickCount);
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private bool _isTerminated;
        private Thread _timerThread;
        private bool _isPaused;
        private readonly Stopwatch _timer = Stopwatch.StartNew();
    }
}
