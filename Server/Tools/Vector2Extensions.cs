using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Wob.Server.Tools
{
    public static class Vector2Extensions
    {
        public static Vector2 ClampMagnitude(this Vector2 original, float maxMagnitude)
        {
            if (original.LengthSquared() > maxMagnitude * maxMagnitude)
                return Vector2.Normalize(original) * maxMagnitude;
            else
                return original;
        }

        public static Vector2i ToMapPosition(this Vector2 position)
        {
            return new Vector2i((int)position.X, (int)position.Y);
        }

        public static Vector2 ToObjectPosition(this Vector2i position)
        {
            return new Vector2(position.X, position.Z);
        }

        public static Vector2 ToVector(this ProtoVector2 position)
        {
            return new Vector2(position.X, position.Y);
        }

        public static ProtoVector2 ToProtoVector(this Vector2 position)
        {
            return new ProtoVector2(position.X, position.Y);
        }
    }
}
