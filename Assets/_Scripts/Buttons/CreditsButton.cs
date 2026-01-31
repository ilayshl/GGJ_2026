using UnityEngine;

public class CreditsButton : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ClickEnter()
    {
        _gameObject.SetActive(true);
    }

    public void ClickExit()
    {
        _gameObject.SetActive(false);
    }
}
