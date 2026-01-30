using UnityEngine;

public class SwitchMusicButton : MonoBehaviour
{
    public void OnPress()
    {
        AudioManager.ChangeMusic();
    }
}
