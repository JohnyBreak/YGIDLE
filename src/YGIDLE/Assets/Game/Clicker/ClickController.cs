using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Clicker
{
    public class ClickController : MonoBehaviour
{
    [SerializeField] private ClickInput _input;
    [SerializeField] private ZoneView _mainZoneView;
    [SerializeField] private ClickAnimation _clickAnimation;
    
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
        _critSystem._zoneManager = _zoneManager;
        
        _onCrit
            .Subscribe(_ => 
            {
                _clickAnimation.Play();
                Debug.LogError("crit");
            }).AddTo(this);
        
        _onIdleClick
            .Subscribe(_ => 
            {
                _clickAnimation.Play();
                Debug.LogError("idle");
            }).AddTo(this);
        BindStreams();
    }

    private void BindStreams()
    {
        var stream = _cooldown.Apply(_input.OnClick);

        stream.Subscribe(pos =>
        {
            var zone = _zoneManager.GetZone(pos);
            if (zone == null) return;

            switch (zone.Type)
            {
                case ZoneType.Crit:
                    _critSystem.TryHit(pos);
                    break;
                
                case ZoneType.Idle:
                    _onIdleClick.OnNext(Unit.Default);
                    if(!_critSystem.IsActive)
                    {
                        if (Random.value < _critChance)
                        {
                            _critSystem.Spawn(_mainZoneView.RandomPoint(Vector3.one * _critSize), _critSize);
                        }
                    }
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