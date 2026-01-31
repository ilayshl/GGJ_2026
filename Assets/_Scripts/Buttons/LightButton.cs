using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LightButton : MonoBehaviour
{
    public bool CanActivate = true;

    [SerializeField] private Sprite brokenLight;

    private Button _button;

    public void OnPress()
    {
        if(CanActivate)
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        else
        {
            GetComponent<Image>().sprite = brokenLight;
            GetComponent<Button>().interactable = false;
        }
    }

    public void DisableButton()
    {
         CanActivate = false;
    }
}
