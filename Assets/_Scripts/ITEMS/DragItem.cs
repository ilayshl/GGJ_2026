using UnityEngine;
using UnityEngine.InputSystem;

public class DragItem : MonoBehaviour
{
    [SerializeField] private Texture2D cursorRegular;
    [SerializeField] private Texture2D cursorHold;
    public Vector2 cursorHotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;
    [SerializeField] private Camera cam;
    private PickableItem pickedObject;
    private Vector3 offset;
    private bool isHeld;
    
    void Update()
    {
       
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider && hit.collider.TryGetComponent<PickableItem>(out PickableItem item))
            {
                if(!item.isOn) return;
                
                pickedObject= hit.collider.GetComponent<PickableItem>();
                offset = pickedObject.transform.position - (Vector3)mouseWorldPos;
                isHeld = true;
            }
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if(pickedObject != null)
            {
            isHeld = false;
                pickedObject.ReturnToOrigin();
            }
                
        }

        if (isHeld)
        {
            pickedObject.transform.position = (Vector3)mouseWorldPos + offset;
           
          
            
        }
    }
}
