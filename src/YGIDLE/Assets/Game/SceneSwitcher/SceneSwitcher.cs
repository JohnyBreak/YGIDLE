using AppInit.InitSteps;
using AppInit.InitSteps.Signals;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneSwitcher
{
    private readonly SignalBus _signalBus;

    public SceneSwitcher(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void Init()
    {
        _signalBus.Subscribe<InitGroupCompletedSignal>(OnSceneInitReady);
    }

    private void OnSceneInitReady(InitGroupCompletedSignal signal)
    {
        if (signal.GroupId == InitStepGroup.StartScene)
        {
            SceneManager.LoadScene("CoreScene");
        }
    }
}
