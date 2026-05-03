using AppInit.InitSteps;
using Source.Scripts.AppInit;
using Zenject;

public class StartSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<TestInitStep1>().AsSingle();
        Container.BindInterfacesAndSelfTo<TestInitStep15>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<SceneSwitcher>().AsSingle();
        Container.BindInterfacesAndSelfTo<SceneSwitcherInitStep>().AsSingle();
        
        Container
            .BindInterfacesAndSelfTo<SceneReadyNotifier>()
            .AsSingle()
            .WithArguments(InitStepGroup.StartScene)
            .NonLazy();

        Container.BindExecutionOrder<SceneReadyNotifier>(int.MaxValue);
    }
}
