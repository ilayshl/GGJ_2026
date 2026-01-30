using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static float SfxVolume {get; private set;} = 1;
    public static float MusicVolume {get; private set;} = 1;
    public static float DialogueVolume {get; private set;} = 1;

    private static SettingsManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public static void SetVolume(SoundType type, float value)
    {
        switch (type)
        {
            case SoundType.SFX:
                SfxVolume = value;
                return;
            case SoundType.Music:
                MusicVolume = value;
                return;
            case SoundType.Dialogue:
                DialogueVolume = value;
                return;
        }
    }
    
    public static float GetVolume(SoundType type)
    {
        switch (type)
        {
            case SoundType.SFX:
                return SfxVolume;
            case SoundType.Music:
                return MusicVolume;
            case SoundType.Dialogue:
                return DialogueVolume;
            default:
            return 0;
        }
    }
}
