using UnityEngine;

public class MaskOnAnimation : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(SetMasked), 2f);
    }

    public void SetMasked()
    {
        FindFirstObjectByType<FaceRenderer>().SetMasked();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
