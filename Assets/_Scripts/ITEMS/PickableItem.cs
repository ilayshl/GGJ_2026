using _Scripts;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public bool isOn;
    public ItemType type;

    [SerializeField] private SpriteRenderer _outline;
    private bool _inCollider = false;
    
    public Vector3 OriginalPosition { get; private set; }

    void Awake()
    {
        OriginalPosition = transform.position;
        if(!isOn)
        {
            _outline.gameObject.SetActive(false);
        }
    }

    public void Drop()
    {
        if(_inCollider)
        {
            //Play Sequence
        }
        else
        {
            ToggleOutline();
            ReturnToOrigin();
        }
    }

    public void ReturnToOrigin()
    {
        transform.position = OriginalPosition;
    }

    public void TogglePickable()
    {
        isOn = !isOn;
        ToggleOutline();
    }

    public void ToggleOutline()
    {
        _outline.gameObject.SetActive(!_outline.gameObject.activeSelf);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Client"))
        {
            _inCollider = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(_inCollider)
        {
            if(collision.CompareTag("Client"))
            {
                _inCollider = false;
            }
        }
    }
}
