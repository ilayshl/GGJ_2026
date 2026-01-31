using System.Collections;
using UnityEngine;

public class GlitchOn : MonoBehaviour
{
    public static GlitchOn Instance;
    
    private bool isGlitchOn;
    private float timeOfGlitch;


    public void setGlitchOn(float num)
    {
        isGlitchOn = true;
        timeOfGlitch = num;
    }
    
    [Header("Glitch Visual")]
    [SerializeField] private GameObject glitchImage;
    
    private Coroutine glitchRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        glitchImage.SetActive(false);
    }

    public void PlayGlitch(float duration)
    {
        if (glitchRoutine != null)
            StopCoroutine(glitchRoutine);

        glitchRoutine = StartCoroutine(GlitchTimer(duration));
    }

    private IEnumerator GlitchTimer(float duration)
    {
        glitchImage.SetActive(true);

        yield return new WaitForSeconds(duration);

        glitchImage.SetActive(false);
        glitchRoutine = null;
    }
}
