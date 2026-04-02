using System;
using Game.BigNumberSpace;
using Game.Clicker.AutoClicker;
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

        [Header("Settings")] [SerializeField] private float _clicksPerSecond = 5f;
        [SerializeField] private float _critChance = 0.2f;
        [SerializeField] private float _critSize = 1f;

        private ZoneManager _zoneManager;
        private CriticalSystem _critSystem;
        private ClickCooldown _cooldown;
        private AutoClickerSystem _autoClicker;

        private Subject<float> _onIdleClick = new Subject<float>();
        private Subject<float> _onUIClick = new Subject<float>();
        private Subject<float> _onCrit = new Subject<float>();

        public IObservable<float> OnIdleClick => _onIdleClick;
        public IObservable<float> OnCrit => _onCrit;
        [Header("Debug")] [SerializeField] private float _resources;

        private void Start()
        {
            _zoneManager = new ZoneManager();
            _zoneManager.InitializeFromScene();
            _autoClicker = new AutoClickerSystem();
            _critSystem = new CriticalSystem();
            _cooldown = new ClickCooldown(_clicksPerSecond);
            _critSystem._zoneManager = _zoneManager;

            _autoClicker.Start();
            _autoClicker.OnAutoClick.Subscribe(value =>
            {
                _clickAnimation.Play();
                AddResources(value);
                Debug.LogError($"auto {_autoClicker.GetTotalCPS()}");
            }).AddTo(this);

            _onCrit
                .Subscribe(value =>
                {
                    _clickAnimation.Play();
                    AddResources(value);
                    Debug.LogError("crit");
                }).AddTo(this);

            _onIdleClick
                .Subscribe(value =>
                {
                    _clickAnimation.Play();
                    AddResources(value);
                    Debug.LogError("idle");
                }).AddTo(this);
            BindStreams();

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.S))
                .Subscribe(_ => _autoClicker.AddClicker(new AutoClickerConfig() { ClicksPerSecond = .1f }))
                .AddTo(this);

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.W))
                .Subscribe(_ => _autoClicker.AddClicker(new AutoClickerConfig() { ClicksPerSecond = .25f }))
                .AddTo(this);

        }

        private void AddResources(float value)
        {
            _resources += value;
            Debug.LogError($"resources {_resources}");
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
                        _onIdleClick.OnNext(1);
                        if (!_critSystem.IsActive)
                        {
                            if (Random.value < _critChance)
                            {
                                _critSystem.Spawn(_mainZoneView.RandomPoint(Vector3.one * _critSize), _critSize);
                            }
                        }

                        break;

                    case ZoneType.UI:
                        _onUIClick.OnNext(0);
                        break;
                }
            }).AddTo(this);

            _critSystem.OnCrit
                .Subscribe(_ => _onCrit.OnNext(250))
                .AddTo(this);
        }
    }
}