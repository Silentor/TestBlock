using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silentor.TB.Client.Config;

namespace Silentor.TB.Client.Tests.Support
{
    public class TestGameConfig : IGameConfig
    {
        public int ChunkViewRadius { get; set; }
        public int ChunkCacheSize { get; set; }
    }
}
