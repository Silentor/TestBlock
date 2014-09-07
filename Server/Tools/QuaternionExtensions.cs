using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Silentor.TB.Common.Network.Messages;

namespace Wob.Server.Tools
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
