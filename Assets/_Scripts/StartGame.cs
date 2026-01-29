using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class StartGame : MonoBehaviour
{
    [SerializeField] private Button Button;
    public void OnStartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
