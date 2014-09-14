using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModestTree;
using ModestTree.Zenject;
using NLog;
using Silentor.TB.Client.Config;
using Silentor.TB.Client.Input;
using Silentor.TB.Client.Tools;
using UnityEngine;

namespace Silentor.TB.Client.Input
{
    public class Input : IInput
    {
        public Input(IApplicationEvents script, IInputConfig config)
        {
            _config = config;

            _script = script;
            _script.GameTick += OnEventsUpdated;
            _script.FrameTick += OnControlUpdated;
            _script.Focused += OnFocused;

            _oldMousePosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }

        /// <summary>
        /// Fires when player changed movement (velocity, rotation, jump)
        /// </summary>
        public event Action<Vector3, Vector2, bool> Moved;

        private void OnFocused(bool state)
        {
            _isFocused = state;
        }

        private void OnControlUpdated()
        {
            if (!_isFocused) return;

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
                _jumped = true;
        }

        private void OnEventsUpdated()
        {
            if (!_isFocused) return;

            //var mousePos = UnityEngine.Input.mousePosition;
            //if(mousePos.x < 0 || mousePos.y < 0 || mousePos.x > Screen.width || mousePos.y > Screen.height)
            //    return;

            var rotation = GetDeltaRotation();

            _moveInput = new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0, UnityEngine.Input.GetAxis("Vertical"));

            if (_moveInput != _oldInput || _jumped || rotation != _oldRotation)
            {
                if(Log.IsTraceEnabled && _jumped) Log.Trace("Pressed Jump key");
                if (Log.IsTraceEnabled && rotation != Vector2.zero) Log.Trace("Rotated for {0}", rotation);
                if (Log.IsTraceEnabled && _moveInput != _oldInput) Log.Trace("Changed move to {0}", _moveInput);

                DoMoved(_moveInput, rotation, _jumped);

                _jumped = false;
                _oldInput = _moveInput;
                _oldRotation = rotation;
            }
        }

        /// <summary>
        /// Get delta rotation values (euler degrees)
        /// </summary>
        /// <returns></returns>
        private Vector2 GetDeltaRotation()
        {
            var newMouse = UnityEngine.Input.mousePosition;
            if (newMouse != _oldMousePosition)
            {
                var delta = newMouse - _oldMousePosition;
                var yaw = delta.x/Screen.width * _config.YawSensitivity;
                var pitch = delta.y/Screen.height * _config.PitchSensitivity;

                _oldMousePosition = newMouse;

                return new Vector2(yaw, pitch);
            }

            return Vector2.zero;
        }

        private void DoMoved(Vector3 movement, Vector2 rotation, bool jump = false)
        {
            if (Moved != null)
                Moved(movement, rotation, jump);
        }

        private readonly IApplicationEvents _script;
        private readonly IInputConfig _config;
        private Vector3 _oldInput;

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private bool _jumped;
        private Vector3 _moveInput;

        private Vector3 _oldMousePosition;
        private Vector2 _oldRotation;
        private bool _isFocused = true;
    }
}
