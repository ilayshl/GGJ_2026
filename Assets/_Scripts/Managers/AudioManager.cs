using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource dialogue;

    [Space]
    [SerializeField] private AudioClip mainMusicLoop;
    [SerializeField] private AudioClip scaryMusicLoop;
    [SerializeField] private AudioClip transitionSound;

   private void Awake()
   {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
   }

    void Start()
    {
        Play(SoundType.Music, mainMusicLoop);
    }

    public static void Play(SoundType type, AudioClip clip)
    {
        switch(type)
        {
        case SoundType.SFX:
            instance.sound.PlayOneShot(clip);
        return;
        case SoundType.Music:
            instance.music.Stop();
            instance.music.clip = clip;
            instance.music.Play();
        return;
        case SoundType.Dialogue:
            instance.dialogue.Stop();
            instance.dialogue.clip = clip;
            instance.dialogue.Play();
        return;    
        }
    }

    public static void UpdateVolume()
    {
            instance.sound.volume = SettingsManager.SfxVolume;
            instance.music.volume = SettingsManager.MusicVolume;
            instance.dialogue.volume = SettingsManager.DialogueVolume;
    }

    public static void ChangeMusic()
    {
        Play(SoundType.SFX, instance.transitionSound);
        float currentTime = instance.music.time;
        Play(SoundType.Music, instance.GetMusicToPlay());
        instance.music.time = currentTime;
    }

    private AudioClip GetMusicToPlay()
    {
        if(instance.music.clip == mainMusicLoop) return scaryMusicLoop;
        else return mainMusicLoop;
    }

    
}

public enum SoundType
{
    SFX,
    Music,
    Dialogue,
}
