using Silentor.TB.Common.Network.Messages;
using UnityEngine;

namespace Silentor.TB.Client.Tools
{
    public static class QuaternionExtensions
    {
        public static ProtoQuaternion ToProtoQuaternion(this Quaternion quaternion)
        {
            return new ProtoQuaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        public static Quaternion ToQuaternion(this ProtoQuaternion quaternion)
        {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

    }
}
