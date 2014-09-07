using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silentor.TB.Client.Tools;

namespace Silentor.TB.Client.Tests
{
    public class TestApplicationEvents : IApplicationEvents
    {
        public event Action FrameTick;
        public event Action GameTick;
        public event Action<bool> Focused;
        public event Action Closed;

        public void DoFrameTick(int count = 1)
        {
            for (var i = 0; i < count; i++)
                if (FrameTick != null)
                    FrameTick();
        }

        public void DoGameTick(int count = 1)
        {
            for (var i = 0; i < count; i++)
                if (GameTick != null)
                    GameTick();
        }

    }
}
