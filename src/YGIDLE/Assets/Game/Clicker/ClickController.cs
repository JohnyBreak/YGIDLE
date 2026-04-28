using System;
using System.Collections.Generic;
using Game.Clicker.AutoClicker;
using Game.Clicker.Ui;
using TMPro;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Clicker
{
    public class ClickController : MonoBehaviour
    {
        [SerializeField] private float _maxResources;
        [SerializeField] private TMP_Text _resourcesText;
        [SerializeField] private ClickInput _input;
        [SerializeField] private ZoneView _mainZoneView;
        [SerializeField] private ClickAnimation _clickAnimation;

        [Header("Settings")] 
        [SerializeField] private float _clicksPerSecond = 5f;
        [SerializeField] private float _critChance = 0.2f;
        [SerializeField] private float _critSize = 1f;
        [SerializeField] private UpgradesView _upgradesView;
        
        private ZoneManager _zoneManager;
        private CriticalSystem _critSystem;
        private ClickCooldown _cooldown;
        private AutoClickerSystem _autoClicker;
        private float _resources;
        
        private Subject<float> _onIdleClick = new Subject<float>();
        private Subject<float> _onUIClick = new Subject<float>();
        private Subject<float> _onCrit = new Subject<float>();

        public bool IsFull => (_maxResources - _resources) < 0.0001f;
        public IObservable<float> OnIdleClick => _onIdleClick;
        public IObservable<float> OnCrit => _onCrit;

        
        List<AutoClickerConfig> _upgrades = new List<AutoClickerConfig>()
        {
            new AutoClickerConfig() { Id = "rat",  ClicksPerSecond = .1f },
            new AutoClickerConfig() { Id = "raven", ClicksPerSecond = .25f },
            new AutoClickerConfig() { Id = "skeleton", ClicksPerSecond = .5f }
        };
        
        private void Start()
        {
            _zoneManager = new ZoneManager();
            _zoneManager.InitializeFromScene();
            _autoClicker = new AutoClickerSystem();
            _critSystem = new CriticalSystem();
            _cooldown = new ClickCooldown(_clicksPerSecond);
            
            _critSystem._zoneManager = _zoneManager;
            
            _autoClicker.IsFull = () => IsFull;
            
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

            _upgradesView.Init(_upgrades);
            _upgradesView.Click
                .Subscribe(id => { _autoClicker.AddClicker(_upgrades[id]); })
                .AddTo(this);
            
            
            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(1))
                .Subscribe(pos =>
                {
                    _resources -= 40;
                    if (_resources < 0)
                    {
                        _resources = 0;
                    }
                })
                .AddTo(this);
        }

        private void AddResources(float value)
        {
            if (IsFull)
            {
                return;
            }

            _resources += value;
            
            if (_resources > _maxResources)
            {
                _resources = _maxResources;
            }
            
            _resourcesText.text = _resources.ToString();
            //Debug.LogError($"resources {_resources}");
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