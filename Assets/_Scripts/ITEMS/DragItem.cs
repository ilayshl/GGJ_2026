using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragItem : MonoBehaviour
{
    [SerializeField] private Texture2D cursorRegular;
    [SerializeField] private Texture2D cursorHold;
    public Vector2 cursorHotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;
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
            Debug.Log("casting ray");

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
            isHeld = false;
                pickedObject.ReturnToOrigin();
                
        }

        if (isHeld)
        {
            pickedObject.transform.position = (Vector3)mouseWorldPos + offset;
           
          
            
        }
    }
}
