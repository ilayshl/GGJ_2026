using UnityEngine;

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

}
