using System;
using UnityEngine;

public class FlipSprite : MonoBehaviour
{
    [SerializeField] private Sprite onTable;

    [SerializeField] private Sprite Onhand;

    [SerializeField] DragItem _drag;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (_drag.isHeld && _drag.pickedObject == this)
        {
            sr.sprite = Onhand;
        }
        else
        {
            sr.sprite = onTable;
        }
    }
}
