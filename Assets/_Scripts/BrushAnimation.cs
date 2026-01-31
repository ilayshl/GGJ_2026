using UnityEngine;

public class BrushAnimation : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(SetSoapy), 2f);
    }

    public void SetSoapy()
    {
        FindFirstObjectByType<FaceRenderer>().SetSoapy();
    }
}
