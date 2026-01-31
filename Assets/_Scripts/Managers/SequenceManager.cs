using System;
using _Scripts;
using UnityEngine;


public class SequenceManager : MonoBehaviour
{
    private static SequenceManager _instance;

    public static SequenceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SequenceManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }


    public void PlaySequence(DialogueGroup groupSequence, GameObject item)
    {
        Animator animator = item.GetComponent<Animator>();
        animator.SetTrigger(groupSequence.groupName);

        DialogueManager.Instance.PlayDialogueGroup(groupSequence);
    }
}