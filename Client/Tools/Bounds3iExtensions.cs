using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Silentor.TB.Common.Maps.Geometry;
using UnityEngine;

namespace Silentor.TB.Client.Tools
{
    public static class Bounds3iExtensions
    {
        public static Bounds ToUnityBounds(this Bounds3i bounds)
        {
            var min = bounds.Min.ToObjectPosition();
            var max = new Vector3(bounds.Max.X + 1, bounds.Max.Y + 1, bounds.Max.Z + 1);
            return new Bounds {min = min, max = max};
        }
    }
}
