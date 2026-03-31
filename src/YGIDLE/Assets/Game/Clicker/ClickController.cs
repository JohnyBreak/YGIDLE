using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Clicker
{
    public class ClickController : MonoBehaviour
{
    [SerializeField] private ClickInput _input;

    [Header("Settings")]
    [SerializeField] private float _clicksPerSecond = 5f;
    [SerializeField] private float _critChance = 0.2f;
    [SerializeField] private float _critSize = 1f;

    private ZoneManager _zoneManager;
    private CriticalSystem _critSystem;
    private ClickCooldown _cooldown;

    private Subject<Unit> _onIdleClick = new Subject<Unit>();
    private Subject<Unit> _onUIClick = new Subject<Unit>();
    private Subject<Unit> _onCrit = new Subject<Unit>();

    public IObservable<Unit> OnIdleClick => _onIdleClick;
    public IObservable<Unit> OnUIClick => _onUIClick;
    public IObservable<Unit> OnCrit => _onCrit;

    private void Start()
    {
        _zoneManager = new ZoneManager();
        _zoneManager.InitializeFromScene();

        _critSystem = new CriticalSystem();
        _cooldown = new ClickCooldown(_clicksPerSecond);

        _onIdleClick
            .Subscribe(_ => 
            {
                Debug.LogError("idle");
            }).AddTo(this);
        BindStreams();
    }

    private void BindStreams()
    {
        var stream = _cooldown.Apply(_input.OnClick);

        stream.Subscribe(pos =>
        {
            // 1. Проверка крита (высший приоритет)
            if (_critSystem.IsActive)
            {
                _critSystem.TryHit(pos);
                return;
            }

            // 2. Получаем зону
            var zone = _zoneManager.GetZone(pos);
            if (zone == null) return;

            switch (zone.Type)
            {
                case ZoneType.Idle:
                    _onIdleClick.OnNext(Unit.Default);

                    if (Random.value < _critChance)
                        _critSystem.Spawn(pos, _critSize);

                    break;

                case ZoneType.UI:
                    _onUIClick.OnNext(Unit.Default);
                    break;
            }
        }).AddTo(this);

        _critSystem.OnCrit
            .Subscribe(_ => _onCrit.OnNext(Unit.Default))
            .AddTo(this);
    }
}
}