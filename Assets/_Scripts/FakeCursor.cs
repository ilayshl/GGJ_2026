using UnityEngine;
using UnityEngine.InputSystem;

public class FakeCursor : MonoBehaviour
{
    [SerializeField] private Sprite defaultHand;
    [SerializeField] private Sprite HoldHand;
    [SerializeField] private Sprite BloodyHand;
    
    
    
    [SerializeField] private Camera cam;

    private SpriteRenderer sr;
    private Vector2 hotspotPixelOffset;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        Sprite s = sr.sprite;

        Vector2 size = s.rect.size;
        Vector2 pivot = s.pivot;
        
        hotspotPixelOffset = new Vector2(
            pivot.x - 10,
            -300
        );
    }


    void Update()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorld.z = 0f;

        transform.position = mouseWorld + (Vector3)(hotspotPixelOffset / sr.sprite.pixelsPerUnit);
    }

    public void SetCursor(int num)
    {
        switch (num)
        {
            case 1:
                sr.sprite = defaultHand;
                break;
            case 2:
                sr.sprite = HoldHand;
                break;
            case 3:
                sr.sprite = BloodyHand;
                break;
                    
        }
    }
   
}