using System;
using System.Threading;
using Silentor.TB.Client.Tools;

namespace Silentor.TB.Client.Console
{
    internal class ApplicationEvents : IApplicationEvents
    {
        public event Action FrameTick;

        public event Action GameTick;

        public event Action<bool> Focused;

        public event Action Closed;

        public ApplicationEvents()
        {
            _timer = new Timer(TickCallback, null, 500, 1000);
        }

        private void TickCallback(object state)
        {
            if (FrameTick != null)
                FrameTick();

            if (GameTick != null)
                GameTick();
        }

        private Timer _timer;
    }
}