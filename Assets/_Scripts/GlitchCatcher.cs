using UnityEngine;

public class GlitchCatcher : MonoBehaviour
{
    private int _counter;
    private FaceRenderer _face;
    void Awake()
    {
        _face = FindFirstObjectByType<FaceRenderer>();
        Debug.Log("GlitchCatcher activated");
    }

    void OnEnable()
    {
        DialogueManager.OnDialogueEndGlitch += CatchGlitch;
        Debug.Log("Subscribed");
    }

    void Start()
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
        if(_counter == 1)
        {
            _face.SetBruised();
        }

        if(_counter == 2)
        {
            _face.SetNormal();
        }
    }
}
