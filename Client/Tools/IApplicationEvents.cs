using System;

namespace Silentor.TB.Client.Tools
{
    public interface IApplicationEvents
    {
        /// <summary>
        /// Fired on every frame rendered
        /// </summary>
        event Action FrameTick;

        /// <summary>
        /// Fixed 10 times per second
        /// </summary>
        event Action GameTick;

        /// <summary>
        /// Fires when application gets or loses focus
        /// </summary>
        event Action<bool> Focused;

        /// <summary>
        /// Fires when applciation closed
        /// </summary>
        event Action Closed;
    }
}