using UnityEngine;

public class GlitchCatcher : MonoBehaviour
{
    private int _counter;
    private FaceRenderer _face;
    void Awake()
    {
        _face = FindFirstObjectByType<FaceRenderer>();
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
        if (_counter == 1 || _counter == 3)
        {
            _face.SetBruised();
        }

        if (_counter == 2 || _counter == 4)
        {
            _face.SetNormal();
        }
    }
}
