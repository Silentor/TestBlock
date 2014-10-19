using System;
using ModestTree.Zenject;
using Silentor.TB.Client.Config;
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
            Container.Bind<IInput>().ToSingle<Input.Input>();
            Container.Bind<IInputConfig>().To(Settings);
        }
    }
}
