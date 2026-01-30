using UnityEngine;

public class PickableItemLocation : MonoBehaviour
{
    public Vector3 OriginalPosition
    
    { get; private set; }

    void Awake()
    {
        OriginalPosition = transform.position;
    }

    public void ReturnToOrigin()
    {
        transform.position = OriginalPosition;
    }
}
