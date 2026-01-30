using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public void OnPress()
    {
        SettingsManager.ToggleSettingsPopup();
    }
}
