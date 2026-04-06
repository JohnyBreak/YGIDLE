using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ClickAnimation : MonoBehaviour
{
    [SerializeField] private float _scaleDownDuration = 0.15f;
    [SerializeField] private float _scaleUpDuration = 0.15f;
    [SerializeField] private float _scaleDownStrength = 0.2f;
    [SerializeField] private float _scaleUpStrength = 0.2f;
    [SerializeField] private float _scaleBaseDuration = 0.15f;
    
    private const float _delay = 0.05f;
    private WaitForSeconds _wait = new(_delay);
    private Vector3 _baseScale;
    private Coroutine _waitRoutine;
    
    private void Awake()
    {
        _baseScale = transform.localScale;
    }

    public void Play()
    {
        if (_waitRoutine != null)
        {
            return;
        }

        transform.DOKill();

        transform.localScale = _baseScale;

        transform.DOScale(_baseScale * _scaleDownStrength, _scaleDownDuration)
            .OnComplete(() =>
            {
                transform.DOScale(_baseScale * _scaleUpStrength, _scaleUpDuration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => { transform.DOScale(_baseScale, _scaleBaseDuration); });
            });

        _waitRoutine = StartCoroutine(WaitRoutine());
    }

    private IEnumerator WaitRoutine()
    {
        yield return _wait;
        _waitRoutine = null;
    }
}