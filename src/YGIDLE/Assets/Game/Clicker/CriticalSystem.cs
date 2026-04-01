using UniRx;
using UnityEngine;
using System;

namespace Game.Clicker
{
    public class CriticalSystem
    {
        public ZoneManager _zoneManager;
        private ClickZone _critZone;
        private ZoneView _zoneView;
        private bool _active;

        private Subject<Unit> _onCrit = new Subject<Unit>();
        public IObservable<Unit> OnCrit => _onCrit;

        public bool IsActive => _active;

        public void Spawn(Vector2 position, float size)
        {
            var bounds = new Bounds(position, Vector3.one * size);
            if (!_zoneView)
            {
                _zoneView = new GameObject().AddComponent<ZoneView>();
            }

            if (_critZone == null)
            {
                _critZone = new ClickZone(ZoneType.Crit, bounds, 100);
                _zoneManager.AddZone(_critZone);
            }
            else
            {
                _critZone.Init(ZoneType.Crit, bounds, 100);
            }
            
            _zoneView.Init(ZoneType.Crit, bounds);
            _zoneView.gameObject.SetActive(true);
            _active = true;
        }

        public void TryHit(Vector2 pos)
        {
            if (!_active) return;

            if (_critZone.Contains(pos))
            {
                _active = false;
                _zoneView.gameObject.SetActive(false);
                _onCrit.OnNext(Unit.Default);
            }
        }
    }
}