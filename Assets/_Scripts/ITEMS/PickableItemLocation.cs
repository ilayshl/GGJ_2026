using UnityEngine;

public class PickableItemLocation : MonoBehaviour
{
    public bool isOn;

    [SerializeField] private SpriteRenderer _outline;
    
    public Vector3 OriginalPosition { get; private set; }

    void Awake()
    {
        OriginalPosition = transform.position;
        if(!isOn)
        {
            _outline.gameObject.SetActive(false);
        }
    }

    public void ReturnToOrigin()
    {
        transform.position = OriginalPosition;
    }

    public void TogglePickable()
    {
        isOn = !isOn;
        _outline.gameObject.SetActive(!_outline.gameObject.activeSelf);
    }
}
