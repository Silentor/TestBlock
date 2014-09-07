using System;
using Assets.Code.Config;
using ModestTree.Zenject;
using Silentor.TB.Client.Input;

namespace Silentor.TB.Client
{
    public class InputModule : Installer
    {
        [Inject]
        private readonly Config Settings;

        [Serializable]
        public class Config : IInputConfig
        {
            public float YawSensitivity = 90;

            public float PitchSensitivity = 90;

            float IInputConfig.YawSensitivity { get { return YawSensitivity; } }

            float IInputConfig.PitchSensitivity { get { return PitchSensitivity; } }
        }

        public override void InstallBindings()
        {
            _container.Bind<IInput>().ToSingle<Input.Input>();
            _container.Bind<IInputConfig>().To(Settings);
        }
    }
}
