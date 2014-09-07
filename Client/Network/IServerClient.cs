using System;
using Silentor.TB.Common.Network.Messages;

namespace Silentor.TB.Client.Network
{
    public interface IServerClient
    {

        /// <summary>
        /// Fires when connected to server
        /// </summary>
        event Action Connected;

        /// <summary>
        /// Fires on succesfull login to server
        /// </summary>
        event Action<LoginResponce> Logined;

        /// <summary>
        /// Fires on chunk received
        /// </summary>
        event Action<ChunkContents> ChunkReceived;

        event Action<EntityUpdate> PositionUpdated;
    }
}
