using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

/// <summary>
/// Responsible for fading in and out an image.
/// </summary>
public class ScreenFade : MonoBehaviour
{
    private static ScreenFade instance;

    public static Action OnEnableFinish;
    public float EnableDuration { get => enableDuration; }
    public float DisableDuration { get => disableDuration; }
    [SerializeField] private bool isActiveOnStart;
    [SerializeField] private float enableDuration;
    [SerializeField] private float disableDuration;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    private bool _isEnabled;
    private Image _image;

    private void Awake()
    {
        instance = this;
        _image = GetComponent<Image>();
    }

    void Start()
    {
            _image.color = startColor;
            if(isActiveOnStart) Fade();
            _isEnabled = isActiveOnStart;
    }

    public static void Fade()
    {
        if (!instance._isEnabled)
        {
            instance._image.DOColor(instance.endColor, instance.disableDuration).OnComplete(() => OnEnableFinish?.Invoke()).SetEase(Ease.InOutSine);
        }
        else
        {
            instance._image.DOColor(instance.startColor, instance.enableDuration).OnComplete(() => OnEnableFinish?.Invoke())
            .SetEase(Ease.InOutSine);
        }
        instance._isEnabled = !instance._isEnabled;
        instance._image.raycastTarget = instance._isEnabled;
    }
}
