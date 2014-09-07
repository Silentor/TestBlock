using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Silentor.TB.Common.Maps.Geometry;

namespace Silentor.TB.Common.Config
{
    public interface IGlobeConfig
    {
        /// <summary>
        /// Seed of the Globe
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// Bounds of the Globe, Globe chunks must be inside bounds
        /// </summary>
        Bounds2i Bounds { get; }
    }
}
