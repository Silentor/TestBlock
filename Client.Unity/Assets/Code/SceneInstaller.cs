using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Code;
using Assets.Scripts;
using ModestTree.Zenject;
using UnityEngine;

namespace Silentor.TB.Client
{
    /// <summary>
    /// Module bindings
    /// </summary>
    public sealed class SceneInstaller : MonoInstaller
    {
        public SystemModule.Config SystemSettings;
        public GameModule.Config GameSettings;
        public VisualizationModule.Config VisualizationSettings;
        public InputModule.Config InputSettings;

        public override void InstallBindings()
        {
            _container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();

            _container.Bind<IInstaller>().ToSingle<SystemModule>();
            _container.Bind<SystemModule.Config>().To(SystemSettings);

            _container.Bind<IInstaller>().ToSingle<GameModule>();
            _container.Bind<GameModule.Config>().To(GameSettings);

            _container.Bind<IInstaller>().ToSingle<VisualizationModule>();
            _container.Bind<VisualizationModule.Config>().To(VisualizationSettings);

            _container.Bind<IInstaller>().ToSingle<InputModule>();
            _container.Bind<InputModule.Config>().To(InputSettings);
        }
    }
}
