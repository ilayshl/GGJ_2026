using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "DialogData", menuName = "Matan's Scripts/Dialog Data")]
    public class DialogData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Must be unique! Will be used as filename.")]
        public string title = "New Dialog";
        
        [Tooltip("Group/Category for organizing dialogues (e.g., 'Tutorial', 'Chapter1', 'BossDialog')")]
        public string groupName = "Default";
        
        public int sortingNumber = 0;
        
        [Header("Content")]
        public string characterName = "";
        [TextArea(3, 10)]
        public string dialogText = "";
        
        [Header("Timing")]
        public float displayDuration = 3f;
        public float typingSpeedOverride = -1f; // -1 = use default, else override
        
        [Header("Audio")]
        public AudioClip dialogueVoiceLine;
        public AudioClip typingSoundEffect;
        public bool playTypingSound = true;
        
        [Header("Visual Effects")]
        public DialogAnimationType animationType = DialogAnimationType.FadeIn;
        public Color characterNameColor = Color.white;
        
    }
    
    public enum DialogAnimationType
    {
        None,
        FadeIn,
        SlideFromBottom,
        SlideFromTop,
        Pop
    }
}
