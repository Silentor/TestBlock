using Silentor.TB.Common.Config;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Server.Config
{
    public class GlobeConfig : IGlobeConfig
    {
        public GlobeConfig()
        {
            Seed = 666;
            Bounds = new Bounds2i(new Vector2i(0, 0), new Vector2i(100, 100));

        }

        public int Seed { get; private set; }

        public Bounds2i Bounds { get; private set; }
    }
}
