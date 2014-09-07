using System;
using UnityEngine;

namespace Silentor.TB.Client.Input
{
    /// <summary>
    /// Unified user input
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// Fires if movement input changed. Params: movement, rotation, jump
        /// </summary>
        event Action<Vector3, Vector2, bool> Moved;
    }
}