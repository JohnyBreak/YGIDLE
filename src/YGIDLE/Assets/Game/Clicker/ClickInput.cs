using System;
using UniRx;
using UnityEngine;

namespace Game.Clicker
{
    public class ClickInput : MonoBehaviour
    {
        public IObservable<Vector2> OnClick => _clickStream;
        private Subject<Vector2> _clickStream = new Subject<Vector2>();

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;

            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Select(_ => ScreenToWorldFast(Input.mousePosition))
                .Subscribe(pos =>
                {
                    _clickStream.OnNext(pos);
                })
                .AddTo(this);
        }
        
        private Vector2 ScreenToWorldFast(Vector2 screenPos)
        {
            var cam = _camera;

            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect;

            Vector2 bottomLeft = (Vector2)cam.transform.position - new Vector2(width / 2f, height / 2f);

            return bottomLeft + new Vector2(
                screenPos.x / Screen.width * width,
                screenPos.y / Screen.height * height
            );
        }
    }
}