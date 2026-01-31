using System.Collections.Generic;
using _Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class ItemsOrderManager : MonoBehaviour
{
    private static ItemsOrderManager instance;
    public Queue<PickableItem> ItemsOrder {get; private set; } = new();

    [SerializeField] PickableItem[] itemsArray;

    void Awake()
    {
        instance = this;
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
    
    public static void PlaySequence(DialogueGroup group)
    {
        SequenceManager.Instance.PlaySequence(group);
        DialogueManager.OnQueueComplete += instance.OnDialogueFinish;
    }

    private void OnDialogueFinish()
    {
        EnableItem();
        DialogueManager.OnQueueComplete -= instance.OnDialogueFinish;
    }

}
