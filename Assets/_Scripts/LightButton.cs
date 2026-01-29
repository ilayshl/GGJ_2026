using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class LightButton : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            Debug.Log("Clicked light");
        }
        
    }

    public void LightPress()
    {
        //animation of light and sound and scene change 
    }
}
