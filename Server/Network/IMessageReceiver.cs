using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Server.Network
{
    interface IMessageReceiver
    {
        void Receive(Message msg);
    }
}
