using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueSceneLoader : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("DialogueScene", LoadSceneMode.Additive);
    }
}
