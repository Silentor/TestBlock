using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Silentor.TB.Common.Maps.Geometry;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Common.Network.Serialization
{
    public static class NetBufferExtensions
    {
        public static void Write(this NetBuffer buffer, ProtoVector3 vector3)
        {
            buffer.Write(vector3.X);
            buffer.Write(vector3.Y);
            buffer.Write(vector3.Z);
        }

        public static ProtoVector3 ReadVector3(this NetBuffer buffer)
        {
            return new ProtoVector3(buffer.ReadFloat(), buffer.ReadFloat(), buffer.ReadFloat());
        }

        public static void Write(this NetBuffer buffer, ProtoVector2 vector2)
        {
            buffer.Write(vector2.X);
            buffer.Write(vector2.Y);
        }

        public static ProtoVector2 ReadVector2(this NetBuffer buffer)
        {
            return new ProtoVector2(buffer.ReadFloat(), buffer.ReadFloat());
        }

        public static void Write(this NetBuffer buffer, ProtoQuaternion quaternion)
        {
            buffer.Write(quaternion.X);
            buffer.Write(quaternion.Y);
            buffer.Write(quaternion.Z);
            buffer.Write(quaternion.W);
        }

        public static ProtoQuaternion ReadQuaternion(this NetBuffer buffer)
        {
            return new ProtoQuaternion(buffer.ReadFloat(), buffer.ReadFloat(), buffer.ReadFloat(), buffer.ReadFloat());
        }

        public static void Write(this NetBuffer buffer, Vector2i position)
        {
            buffer.Write(position.X);
            buffer.Write(position.Z);
        }

        public static Vector2i ReadVector2i(this NetBuffer buffer)
        {
            return new Vector2i(buffer.ReadInt32(), buffer.ReadInt32());
        }

        public static void Write(this NetBuffer buffer, Vector3i position)
        {
            buffer.Write(position.X);
            buffer.Write(position.Y);
            buffer.Write(position.Z);
        }

        public static Vector3i ReadVector3i(this NetBuffer buffer)
        {
            return new Vector3i(buffer.ReadInt32(), buffer.ReadInt32(), buffer.ReadInt32());
        }
    }
}
