using UnityEngine;

public class GlitchCatcher : MonoBehaviour
{
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
        AudioManager.ChangeMusic();
        _counter++;
        Debug.Log(_counter);
        if (_counter == 1)
        {
            _face.SetBruised();
            _light.DisableButton();
            
        }

        if (_counter == 2)
        {
            _face.SetNormal();
        }

        if(_counter == 3)
        {
            _cursor.SetCursor(3);
            _plant.SwitchPlant(3);
        }

        if(_counter == 4)
        {
            _cursor.SetCursor(1);
            _plant.SwitchPlant(1);
        }
    }
}
