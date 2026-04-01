using UnityEngine;
using DG.Tweening;

public class ClickAnimation : MonoBehaviour
{
    [SerializeField] private float _scaleDownDuration = 0.15f;
    [SerializeField] private float _scaleUpDuration = 0.15f;
    [SerializeField] private float _scaleDownStrength = 0.2f;
    [SerializeField] private float _scaleUpStrength = 0.2f;
    [SerializeField] private float _scaleBaseDuration = 0.15f;

    private Vector3 _baseScale;

    private void Awake()
    {
        _baseScale = transform.localScale;
    }

    public void Play()
    {
        transform.DOKill();

        transform.localScale = _baseScale;

        transform.DOScale(_baseScale * _scaleDownStrength, _scaleDownDuration)
            .OnComplete(() =>
            {
                transform.DOScale(_baseScale * _scaleUpStrength, _scaleUpDuration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => { transform.DOScale(_baseScale, _scaleBaseDuration); });
            });
    }
}