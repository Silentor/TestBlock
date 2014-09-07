using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Silentor.TB.Common.Tools
{
    /// <summary>
    /// Stopwatch timer for measuring average and max time of some repeating process
    /// </summary>
    public class AverageTimer
    {
        public AverageTimer()
        {
            _watch = new Stopwatch();
        }

        /// <summary>
        /// Average time of operation
        /// </summary>
        public float AverageTime { get { return (float)_elapsedTime / _samples; } }

        /// <summary>
        /// Max time of operation
        /// </summary>
        public int MaxTime { get; private set; }

        /// <summary>
        /// Time of last operation
        /// </summary>
        public int LastTime { get; private set; }

        public void StartTimer()
        {
            if (_watch == null)
                _watch = new Stopwatch();
            _watch.Start();
        }

        public AverageTimer StopTimer()
        {
            _watch.Stop();

            var elapsed = (int)(_watch.ElapsedMilliseconds - _elapsedTime);
            if (MaxTime < elapsed) MaxTime = elapsed;
            LastTime = elapsed;

            _elapsedTime = _watch.ElapsedMilliseconds;
            _samples++;

            return this;
        }

        private Stopwatch _watch;
        private int _samples;
        private long _elapsedTime;
    }
}
