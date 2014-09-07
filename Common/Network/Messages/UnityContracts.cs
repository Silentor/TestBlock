using System;
using ProtoBuf;

namespace Silentor.TB.Common.Network.Messages
{
    [ProtoContract]
    public class ProtoVector3
    {
        [ProtoMember(1)]
        public float X { get; set; }

        [ProtoMember(2)]
        public float Y { get; set; }

        [ProtoMember(3)]
        public float Z { get; set; }

        public ProtoVector3()
        {
            this.X = 0.0f;
            this.Y = 0.0f;
            this.Z = 0.0f;
        }

        public ProtoVector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override string ToString()
        {
            return String.Format("Vector3({0:F3}, {1:F3}, {2:F3})", X, Y, Z);
        }
    }

    [ProtoContract]
    public class ProtoVector2
    {
        [ProtoMember(1)]
        public float X { get; set; }

        [ProtoMember(2)]
        public float Y { get; set; }

        public ProtoVector2()
        {
        }

        public ProtoVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return String.Format("Vector2({0:F3}, {1:F3})", X, Y);
        }
    }

    [ProtoContract]
    public class ProtoQuaternion
    {
        [ProtoMember(1)]
        public float X { get; set; }

        [ProtoMember(2)]
        public float Y { get; set; }

        [ProtoMember(3)]
        public float Z { get; set; }

        [ProtoMember(4)]
        public float W { get; set; }

        public ProtoQuaternion()
        {
        }

        public ProtoQuaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public override string ToString()
        {
            return String.Format("Quaternion({0:F3}, {1:F3}, {2:F3}, {3:F3})", X, Y, Z, W);
        }
    }
}
