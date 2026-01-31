using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class StartGame : MonoBehaviour
{
    [SerializeField] private Button Button;
    public void OnStartGame()
    {
        ScreenFade.Fade();
        ScreenFade.OnEnableFinish += StartDialogue;
    }

    private void StartDialogue()
    {
        ScreenFade.OnEnableFinish -= StartDialogue;
        GameManager.StartDialogue("Start");
    }

}
