using UnityEngine;

public class TowelAnimation : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(SetNormal), 2f);
    }

    public void SetNormal()
    {
        FindFirstObjectByType<FaceRenderer>().SetNormal();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void DisablePimples()
    {
        FindFirstObjectByType<Pimples>().gameObject.SetActive(false);
    }
}
