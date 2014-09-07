using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using UnityEngine;

namespace Silentor.TB.Client.Tools
{
    public static class Vector3Extensions
    {
        public static Vector3i ToMapPosition(this Vector3 position)
        {
            return new Vector3i((int)position.x, (int)position.y, (int)position.z);
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
            return new ProtoVector3(position.x, position.y, position.z);
        }
    }
}
