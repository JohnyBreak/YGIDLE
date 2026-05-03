using System;
using AppInit.InitSteps;
using UnityEngine;
using Zenject;

[InitStepOrder(InitStepGroup.StartScene, 10)]
public class SceneSwitcherInitStep : InitStepBase, IInitializable
{
    private readonly IInitStepsProvider _initStepsProvider;
    private readonly SceneSwitcher _sceneSwitcher;
    
    public SceneSwitcherInitStep(
        IInitStepsProvider initStepsProvider,
        SceneSwitcher sceneSwitcher)
    {
        _initStepsProvider = initStepsProvider;
        _sceneSwitcher = sceneSwitcher;
    }

    public void Initialize()
    {
        _initStepsProvider.Register(this);
    }

    protected override void OnStart()
    {
        try
        {
            _sceneSwitcher.Init();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Fail(e);
            return;
        }

        Done();
    }

    public override void Dispose()
    {
        _initStepsProvider.Unregister(this);
        base.Dispose();
    }
}
