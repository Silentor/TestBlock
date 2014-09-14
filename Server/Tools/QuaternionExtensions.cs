using Microsoft.Xna.Framework;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Server.Tools
{
    public static class QuaternionExtensions
    {
        public static ProtoQuaternion ToProtoQuaternion(this Quaternion quaternion)
        {
            return new ProtoQuaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public static Quaternion ToQuaternion(this ProtoQuaternion quaternion)
        {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

    }
}
