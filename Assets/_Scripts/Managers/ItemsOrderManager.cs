using System.Collections.Generic;
using UnityEngine;

public class ItemsOrderManager : MonoBehaviour
{
    public Queue<PickableItem> ItemsOrder {get; private set; } = new();

    [SerializeField] PickableItem[] itemsArray;

    void Awake()
    {
        foreach(var item in itemsArray)
        {
            ItemsOrder.Enqueue(item);
        }
        
    }

    void Start()
    {
        EnableItem();
    }

    public void EnableItem()
    {
        var currentItem = ItemsOrder.Dequeue();
            currentItem.TogglePickable();
    }

}
