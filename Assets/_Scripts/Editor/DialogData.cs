using System.Collections.Generic;
using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "DialogData", menuName = "Matan's Scripts/Dialog Data")]
    public class DialogData : ScriptableObject
    {
        public string title = "New Dialog";
        public int sortingNumber = 0;
        [TextArea(3, 10)]
        public string dialogText = "";
        public string characterName = "";
        public float displayDuration = 3f;
    }
}
