using Silentor.TB.Common.Maps.Geometry;
using UnityEngine;

namespace Silentor.TB.Client.Network
{
    /// <summary>
    /// Client to server connection
    /// </summary>
    public interface IClientServer
    {
        /// <summary>
        /// Request for chunk
        /// </summary>
        /// <param name="position"></param>
        void GetChunk(Vector2i position);

        /// <summary>
        /// Send movement of player
        /// </summary>
        /// <param name="movement"></param>
        /// <param name="jump"></param>
        void MovePlayer(Vector3 movement, Vector2 rotation, bool jump = false);

        /// <summary>
        /// Add new Player to World
        /// </summary>
        void AddPlayer(string name);
    }
}
