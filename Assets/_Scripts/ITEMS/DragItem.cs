using UnityEngine;
using UnityEngine.InputSystem;

public class DragItem : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Camera cam;
    private PickableItemLocation pickedObject;
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

            if (hit.collider && hit.collider.TryGetComponent<PickableItemLocation>(out PickableItemLocation item))
            {
                if(!item.isOn) return;
                
                pickedObject= hit.collider.GetComponent<PickableItemLocation>();
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
