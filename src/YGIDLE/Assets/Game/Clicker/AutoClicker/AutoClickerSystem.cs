using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;

namespace Game.Clicker.AutoClicker
{
    public class AutoClickerSystem
    {
        private List<AutoClickerModel> _clickers = new List<AutoClickerModel>();

        private Subject<float> _onAutoClick = new Subject<float>();
        public IObservable<float> OnAutoClick => _onAutoClick;
        private float _buffer;
        
        public void Start()
        {
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    float cps = GetTotalCPS();
                    _buffer += cps * Time.deltaTime;
                    if (_buffer >= 1f)
                    {
                        int clicks = Mathf.FloorToInt(_buffer);
                        _buffer -= clicks;
                        _onAutoClick.OnNext(clicks);
                    }
                });
            
            // Observable.EveryUpdate()
            //     .Subscribe(_ =>
            //     {
            //         float cps = GetTotalCPS();
            //         if (cps <= 0f) return;
            //
            //         float clicksThisFrame = cps * Time.deltaTime;
            //
            //         _onAutoClick.OnNext(clicksThisFrame);
            //     });
        }
        
        public void AddClicker(AutoClickerConfig config, int amount = 1)
        {
            var existing = _clickers.Find(c => c.Config == config);

            if (existing != null)
            {
                existing.Count += amount;
            }
            else
            {
                _clickers.Add(new AutoClickerModel
                {
                    Config = config,
                    Count = amount
                });
            }
        }

        public float GetTotalCPS()
        {
            float total = 0f;

            foreach (var c in _clickers)
                total += c.TotalCPS;

            return total;
        }
        
        public int GetCost(AutoClickerModel model)
        {
            return Mathf.RoundToInt(model.Config.Cost * Mathf.Pow(1.15f, model.Count));
        }
        
    }
}