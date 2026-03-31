using UniRx;
using System;

namespace Game.Clicker
{
    public class ClickCooldown
    {
        private readonly float _cooldown;

        public ClickCooldown(float clicksPerSecond)
        {
            _cooldown = 1f / clicksPerSecond;
        }

        public IObservable<T> Apply<T>(IObservable<T> source)
        {
            return source.ThrottleFirst(TimeSpan.FromSeconds(_cooldown));
        }
    }
}