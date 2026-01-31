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
    [SerializeField] private PickableItem pickedObject;
    private Vector3 offset;
    private bool isHeld;

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

            if (hit.collider && hit.collider.CompareTag("Pick"))
            {
                pickedObject = hit.collider.GetComponent<PickableItem>();
                if (pickedObject.isOn)
                {
                    pickedObject.ToggleOutline();
                    _fakeCursor.SetCursor(2);
                    offset = pickedObject.transform.position - (Vector3)mouseWorldPos;
                    isHeld = true;
                }
                else
                {
                    pickedObject = null;
                }
            }

        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (pickedObject != null)
            {
                isHeld = false;
                if (pickedObject == null)
                {
                    _fakeCursor.SetCursor(1);
                    return;
                }
                pickedObject.Drop();
                _fakeCursor.SetCursor(1);
                pickedObject = null;
            }

        }

        if (isHeld)
        {
            pickedObject.transform.position = (Vector3)mouseWorldPos + offset;
        }
    }
}