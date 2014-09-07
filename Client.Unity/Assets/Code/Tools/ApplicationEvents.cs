using System;
using Silentor.TB.Client.Tools;
using Silentor.TB.Common.Tools;
using UnityEngine;

namespace Silentor.TB.Client.Tools
{
    /// <summary>
    /// Produced application level events
    /// </summary>
    public class ApplicationEvents : MonoBehaviour, IApplicationEvents
    {
        public event Action GameTick;

        public event Action<bool> Focused;

        public event Action FrameTick;

        public event Action Closed;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                throw new InvalidOperationException("ScriptHost must be singletone");
        }

        void OnDestroy()
        {
            if (Closed != null)
                Closed();
            Instance = null;
        }

        void Update()
        {

            if (FrameTick != null)
                FrameTick();
        }

        void FixedUpdate()
        {
            if (GameTick != null)
                GameTick();
        }

        void OnApplicationFocus(bool state)
        {
            if (Focused != null)
                Focused(state);
        }

        private static ApplicationEvents Instance { get; set; }

        private readonly AverageTimer _OnFrameTimer = new AverageTimer();
        private readonly AverageTimer _FixedTickTimer = new AverageTimer();
    }
}