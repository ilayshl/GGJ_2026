using System.Collections.Generic;
using UnityEngine;

public class ItemsOrderManager : MonoBehaviour
{
    public Queue<PickableItemLocation> ItemsOrder {get; private set; } = new();

    [SerializeField] PickableItemLocation[] itemsArray;

    void Awake()
    {
        foreach(var item in itemsArray)
        {
            ItemsOrder.Enqueue(item);
        }
        EnableItem();
    }


    public void EnableItem()
    {
        var currentItem = ItemsOrder.Peek();
        if(currentItem.TryGetComponent<PickableItemLocation>(out var pickable))
        {
            pickable.ToggleOutline();
        }
    }

}
