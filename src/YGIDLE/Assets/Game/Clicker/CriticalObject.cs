using System;
using UnityEngine;
using UniRx;

namespace Game.Clicker
{
    public class CriticalObject : MonoBehaviour
    {
        private Subject<Unit> _onClicked = new Subject<Unit>();
        public IObservable<Unit> OnClicked => _onClicked;

        private void OnMouseDown()
        {
            _onClicked.OnNext(Unit.Default);
            Destroy(gameObject);
        }
    }
}