using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource dialogue;

   private void Awake()
   {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
   }

   public static void Play(SoundType type, AudioClip clip)
    {
        switch(type)
        {
        case SoundType.SFX:
            instance.sound.volume = SettingsManager.SfxVolume;
            instance.sound.PlayOneShot(clip);
            Debug.Log("SFX Played");
        return;
        case SoundType.Music:
            instance.music.volume = SettingsManager.MusicVolume;
            instance.music.Stop();
            instance.music.clip = clip;
            instance.music.Play();
        return;
        case SoundType.Dialogue:
            instance.dialogue.volume = SettingsManager.DialogueVolume;
            instance.dialogue.Stop();
            instance.dialogue.clip = clip;
            instance.dialogue.Play();
        return;    
        }
    }
}

public enum SoundType
{
    SFX,
    Music,
    Dialogue,
}
