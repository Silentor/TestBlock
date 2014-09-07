using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;
using UnityEngine;

namespace Silentor.TB.Client.Tools
{
    public static class Vector2Extensions
    {
        public static Vector2i ToMapPosition(this Vector2 position)
        {
            return new Vector2i((int)position.x, (int)position.y);
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
            return new ProtoVector2(position.x, position.y);
        }
    }
}
