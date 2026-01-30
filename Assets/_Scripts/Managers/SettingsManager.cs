using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static float SfxVolume { get; private set; } = 1;
    public static float MusicVolume { get; private set; } = 1;
    public static float DialogueVolume { get; private set; } = 1;

    [SerializeField] private Canvas settingsCanvasPrefab;
    private Canvas _currentSettings;

    private static SettingsManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        CreatePopup();
    }

    private void CreatePopup()
    {
        string settingsScene = "SettingsScene";
        SceneManager.LoadSceneAsync(settingsScene, LoadSceneMode.Additive);
        _currentSettings = Instantiate(settingsCanvasPrefab);
        _currentSettings.gameObject.SetActive(false);
        Scene activeSettingsScene = SceneManager.GetSceneByName(settingsScene);
        SceneManager.MoveGameObjectToScene(_currentSettings.gameObject, activeSettingsScene);
    }
    public static void ToggleSettingsPopup()
    {
        instance._currentSettings.gameObject.SetActive(!instance._currentSettings.isActiveAndEnabled);
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
