using AppInit.InitSteps;
using Zenject;

[InitStepOrder(InitStepGroup.StartScene, 1)]
public class TestInitStep1 : InitStepBase, IInitializable
{
    private readonly IInitStepsProvider _initStepsProvider;
    
    public TestInitStep1(
        IInitStepsProvider initStepsProvider)
    {
        _initStepsProvider = initStepsProvider;
    }
    
    public void Initialize()
    {
        _initStepsProvider.Register(this);
    }
    
    protected override void OnStart()
    {
        Done();
    }
    
    public override void Dispose()
    {
        _initStepsProvider.Unregister(this);
        base.Dispose();
    }
}

[InitStepOrder(InitStepGroup.StartScene, 15)]
public class TestInitStep15 : InitStepBase, IInitializable
{
    private readonly IInitStepsProvider _initStepsProvider;
    
    public TestInitStep15(
        IInitStepsProvider initStepsProvider)
    {
        _initStepsProvider = initStepsProvider;
    }

    public void Initialize()
    {
        _initStepsProvider.Register(this);
    }
    
    protected override void OnStart()
    {
        Done();
    }
    
    public override void Dispose()
    {
        _initStepsProvider.Unregister(this);
        base.Dispose();
    }
}