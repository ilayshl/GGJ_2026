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

        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
    }

    public static void StartDialogue(string name)
    {
        DialogueManager.Instance.EnqueueDialoguesByGroup(name);
    }
}
