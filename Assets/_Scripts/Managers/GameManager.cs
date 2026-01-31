using _Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public static void StartDialogue(string name)
    {
        DialogueManager.Instance.EnqueueDialoguesByGroup(name);
        if(name == "Start")
        {
            DialogueManager.OnQueueComplete += instance.FadeInGameScene;
        }
    }

    private void FadeInGameScene()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainMenuScene");
        DialogueManager.OnQueueComplete -= FadeInGameScene;
        ScreenFade.Fade();
    }
}
