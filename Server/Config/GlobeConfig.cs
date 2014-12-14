using System;
using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Server.Config
{
    public class GlobeConfig : IGlobeConfig
    {
        public GlobeConfig()
        {
            Seed = Environment.TickCount;
            Bounds = new Bounds2i(new Vector2i(0, 0), new Vector2i(50, 50));
        }

        public int Seed { get; private set; }

        public Bounds2i Bounds { get; private set; }
    }
}
