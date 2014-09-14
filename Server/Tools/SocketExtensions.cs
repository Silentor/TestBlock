using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Silentor.TB.Server.Tools
{
    public static class SocketExtensions
    {
        public static Task<int> SendTaskAsync(this Socket socket, byte[] buffer)
        {
            return Task.Factory.FromAsync<int>(
                             socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, socket),
                             socket.EndSend);
        }
    }
}
