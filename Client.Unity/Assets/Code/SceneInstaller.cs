using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Container.Bind<IInstaller>().ToSingle<SystemModule>();
            Container.Bind<SystemModule.Config>().To(SystemSettings);

            Container.Bind<IInstaller>().ToSingle<GameModule>();
            Container.Bind<GameModule.Config>().To(GameSettings);

            Container.Bind<IInstaller>().ToSingle<VisualizationModule>();
            Container.Bind<VisualizationModule.Config>().To(VisualizationSettings);

            Container.Bind<IInstaller>().ToSingle<InputModule>();
            Container.Bind<InputModule.Config>().To(InputSettings);

#if UNITY_WEBPLAYER
            if (!Security.PrefetchSocketPolicy(SystemSettings.ServerAddress, 9999, 1000))
                UnityEngine.Debug.LogError("Socket security failed, probably server is down");
#endif
        }
    }
}
