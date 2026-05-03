using AppInit.InitSteps;
using AppInit.InitSteps.Signals;
using Source.Scripts.App;
using Source.Scripts.AppInit;
using UnityEngine;
using Zenject;

namespace Source.Installers
{
    public class AppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            DeclareSignals();

            DeclareInitSteps();
            
            var coroutineRunner = new GameObject("GroupIniterCoroutines").AddComponent<CoroutineRunner>();
            DontDestroyOnLoad(coroutineRunner.gameObject);

            Container.Bind<CoroutineRunner>().FromInstance(coroutineRunner).AsSingle();

            Container.Bind<StepIniter>()
                .ToSelf()
                .FromNew()
                .AsSingle()
                .Lazy();

            Container.BindInterfacesAndSelfTo<AppController>().AsSingle().NonLazy();
            
            Container.Bind<IInitStepsProvider>().To<InitStepProvider>().FromNew().AsSingle().Lazy();
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<AppInitializationStart>();
            Container.DeclareSignal<AppInitializationFinish>();
            Container.DeclareSignal<AppPausedSignal>();
            Container.DeclareSignal<AppQuittingSignal>();
            Container.DeclareSignal<SceneInitReadySignal>();
            Container.DeclareSignal<InitGroupCompletedSignal>();
        }

        private void DeclareInitSteps()
        {
        }
    }
}