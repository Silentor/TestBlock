using Microsoft.Xna.Framework;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Server.Tools
{
    public static class Vector3Extensions
    {
        public static Vector3 ClampMagnitude(this Vector3 original, float maxMagnitude)
        {
            if (original.LengthSquared() > maxMagnitude * maxMagnitude)
                return Vector3.Normalize(original) * maxMagnitude;
            else
                return original;
        }

        public static Vector3i ToMapPosition(this Vector3 position)
        {
            return new Vector3i((int)position.X, (int)position.Y, (int)position.Z);
        }

        public static Vector3 ToObjectPosition(this Vector3i position)
        {
            return new Vector3(position.X, position.Y, position.Z);
        }

        public static Vector3 ToVector(this ProtoVector3 position)
        {
            return new Vector3(position.X, position.Y, position.Z);
        }

        public static ProtoVector3 ToProtoVector(this Vector3 position)
        {
            return new ProtoVector3(position.X, position.Y, position.Z);
        }
    }
}
