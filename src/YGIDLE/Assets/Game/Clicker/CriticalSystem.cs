using UniRx;
using UnityEngine;
using System;

namespace Game.Clicker
{
    public class CriticalSystem
    {
        private ClickZone _critZone;
        private bool _active;

        private Subject<Unit> _onCrit = new Subject<Unit>();
        public IObservable<Unit> OnCrit => _onCrit;

        public bool IsActive => _active;

        public void Spawn(Vector2 position, float size)
        {
            var bounds = new Bounds(position, Vector3.one * size);

            _critZone = new ClickZone(ZoneType.Crit, bounds, 100);
            _active = true;
        }

        public void TryHit(Vector2 pos)
        {
            if (!_active) return;

            if (_critZone.Contains(pos))
            {
                _active = false;
                _onCrit.OnNext(Unit.Default);
            }
        }

        public ClickZone GetZone() => _active ? _critZone : null;
    }
}