using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragItem : MonoBehaviour
{
    [SerializeField] private FakeCursor _fakeCursor;
    public Vector2 cursorHotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;
    [SerializeField] private Camera cam;
    private PickableItemLocation pickedObject;
    private Vector3 offset;
    public bool isHeld;
    

    private void Start()
    {
        _fakeCursor.SetCursor(1);
    }

    void Update()
    {
       
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            Debug.Log("casting ray");
            _fakeCursor.SetCursor(2);
          
            if (hit.collider && hit.collider.CompareTag("Pick"))
            {
                Debug.Log("picked item");
                
                pickedObject= hit.collider.GetComponent<PickableItemLocation>();
                offset = pickedObject.transform.position - (Vector3)mouseWorldPos;
                isHeld = true;
                
            }
            
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            SpriteRenderer sprite = pickedObject.GetComponent<SpriteRenderer>();
            sprite.sortingOrder = 0;
            isHeld = false;
            if (pickedObject == null)
            {
                _fakeCursor.SetCursor(1);
                return;
            }
                pickedObject.ReturnToOrigin();
                _fakeCursor.SetCursor(1);
                
        }

        if (isHeld)
        {
            SpriteRenderer sprite = pickedObject.GetComponent<SpriteRenderer>();
            sprite.sortingOrder = 10;
            pickedObject.transform.position = (Vector3)mouseWorldPos + offset;
            
            
        }
    }
}
