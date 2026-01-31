using _Scripts;
using UnityEngine;

public class GlitchCatcher : MonoBehaviour
{
    [SerializeField] private DialogueGroup finalGroup;
    private int _counter;
    private FaceRenderer _face;
    private FakeCursor _cursor;
    private PlantController _plant;
    private LightButton _light;

    void Awake()
    {
        _face = FindFirstObjectByType<FaceRenderer>();
        _cursor = FindFirstObjectByType<FakeCursor>();
        _plant = FindFirstObjectByType<PlantController>();
        _light = FindFirstObjectByType<LightButton>();

    }

    void OnEnable()
    {
        DialogueManager.OnDialogueEndGlitch += CatchGlitch;
    }

    void OnDisable()
    {
        DialogueManager.OnDialogueEndGlitch -= CatchGlitch;
    }

    private void CatchGlitch()
    {
        Debug.Log("Glitched!");
        GlitchOn.Instance.PlayGlitch();
        _counter++;
        Debug.Log(_counter);
        if (_counter == 1)
        {
            _face.SetBruised();
            _light.DisableButton();
            AudioManager.ChangeMusic();//scary
        }

        if (_counter == 2)
        {
            _face.SetNormal();
                    AudioManager.ChangeMusic();//normal

        }

        if(_counter == 3)
        {
            _cursor.SetCursor(3);
            _plant.SwitchPlant(3);
                    AudioManager.ChangeMusic();//scary
        }

        if(_counter == 4)
        {
            SequenceManager.Instance.PlaySequence(finalGroup);
        }

        if(_counter == 5)
        {
            _cursor.SetCursor(1);
            _plant.SwitchPlant(1);
            AudioManager.ChangeMusic();//normal
        }

        if(_counter == 6)
        {
            _face.SetHalfMaskedBloody();
            _cursor.SetCursor(3);
                        _plant.SwitchPlant(3);
                    AudioManager.ChangeMusic();//scary

        }

        if(_counter == 7)
        {
            _face.SetBloody();
        }

        if(_counter == 8)
        {
            _face.SetHalfMaskedNormal();
            _plant.SwitchPlant(1);
            _cursor.SetCursor(1);
            AudioManager.ChangeMusic();//normal

        }

        if(_counter == 8)
        {
            Invoke(nameof(Fade), 3f);
        }
    }

    private void Fade()
    {
        ScreenFade.Fade();
        _light.CanActivate = true;
        _light.OnPress();
    }
}
