using UnityEngine;
using UnityEngine.UI;

public class PlantController : MonoBehaviour
{
    public static PlantController Instance;

    [Header("Plants Stages")]
    [SerializeField]
    private Sprite Plant1;
    [SerializeField] private Sprite Plant2;
    [SerializeField] private Sprite Plant3;
    [SerializeField] private Image sr;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    public void SwitchPlant(int num)
    {
        switch (num)
        {
            case 1:
                sr.sprite = Plant1;
                break;
            case 2:
                sr.sprite = Plant2;
                break;
            case 3:
                sr.sprite = Plant3;
                break;
                
                
        }
    }
}
