using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderInteraction : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private SoundType mixer;
    [SerializeField] private AudioClip sound;
    [SerializeField] private bool showDecimalPoints;
    [Space]
    [Header("Will fill by itself")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private Slider slider;

    private float _currentValue;

    void Awake()
    {
        slider = GetComponent<Slider>();
        text = GetComponentInChildren<TMP_Text>();
    }

    void Start()
    {
        float value = SettingsManager.GetVolume(mixer);
        HandleOnValueChange(value);
        slider.value = value;
    }

    public void HandleOnValueChange(float value)
    {
        if(showDecimalPoints)
        {
            text.SetText((value * 100).ToString("F2"));
        }
        else
        {
            text.SetText((value * 100).ToString("F0"));
        }
        _currentValue = value;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SettingsManager.SetVolume(mixer, _currentValue);
        if(sound != null)
        {
            AudioManager.Play(mixer, sound);
        }
    }    
}
