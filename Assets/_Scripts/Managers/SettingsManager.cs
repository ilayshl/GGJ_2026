using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static float SfxVolume = 1;
    public static float MusicVolume = 1;
    public static float DialogueVolume = 1;

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

    public void ChangeSfxVolume(float value)
    {
        SetVolume(SoundType.SFX, value);
    }

    public void ChangeMusicVolume(float value)
    {
        SetVolume(SoundType.Music, value);
    }

    public void ChangeDialogueVolume(float value)
    {
        SetVolume(SoundType.Dialogue, value);
    }

    private void SetVolume(SoundType type, float value)
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
}
