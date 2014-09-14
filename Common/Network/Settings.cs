using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace Silentor.TB.Common.Network
{
    public static class Settings
    {
        public const string AppIdentifier = "Silentor.WoB";
        public const int Port = 10000;
        public const int SecurityPort = 9999;

        //System management messages (login, disconnect...)
        public readonly static DeliveryMethod System = new DeliveryMethod(NetDeliveryMethod.ReliableOrdered, 0);

        //Chunk sendings
        public readonly static DeliveryMethod Chunk = new DeliveryMethod(NetDeliveryMethod.ReliableUnordered, 0);

        //Player action (move, jump, attack)
        public readonly static DeliveryMethod PlayerAction = new DeliveryMethod(NetDeliveryMethod.ReliableOrdered, 1);

        //Entity updates around player
        public readonly static DeliveryMethod EntityUpdate = new DeliveryMethod(NetDeliveryMethod.UnreliableSequenced, 0);
    }
}
