using _Scripts;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public bool isOn;
    public ItemType type;

    [SerializeField] private SpriteRenderer _outline;
    [SerializeField] private DialogueGroup _dialogueGroup;
    [SerializeField] private GameObject animationObject;
    private bool _inCollider = false;

    public Vector3 OriginalPosition { get; private set; }

    void Awake()
    {
        OriginalPosition = transform.position;
        if (!isOn)
        {
            _outline.gameObject.SetActive(false);
        }
    }

    public void Drop()
    {
        if (_inCollider)
        {
            Instantiate(animationObject);
            SequenceManager.Instance.PlaySequence(_dialogueGroup, this.gameObject);
            gameObject.SetActive(false);
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
        _outline.gameObject.SetActive(isOn);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Client"))
        {
            Debug.Log("Client entered pickable item");
            _inCollider = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (_inCollider)
        {
            if (collision.CompareTag("Client"))
            {
                Debug.Log("Client exit pickable item");

                _inCollider = false;
            }
        }
    }
}