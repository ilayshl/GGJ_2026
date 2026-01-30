using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(fileName = "DialogueGroup", menuName = "Matan's Scripts/Dialogue Group")]
    public class DialogueGroup : ScriptableObject
    {
        [Header("Group Info")]
        public string groupName = "New Group";
        [TextArea(2, 4)]
        public string description = "";
        
        [Header("Auto-populate Settings")]
        [Tooltip("If enabled, automatically finds all DialogData with matching groupName")]
        public bool autoPopulateFromCategory = false;
        
        [Header("Dialogues in Order")]
        [Tooltip("Dialogues will play in this order when group is triggered")]
        public List<DialogData> dialogues = new List<DialogData>();
        

        public void RefreshDialoguesFromCategory()
        {
            if (!autoPopulateFromCategory) return;
            
            dialogues.Clear();
            
            // Load all DialogData from Resources
            DialogData[] allDialogues = Resources.LoadAll<DialogData>("Dialogues");
            
            // Filter by matching groupName and sort by sortingNumber
            dialogues = allDialogues
                .Where(d => d.groupName == groupName)
                .OrderBy(d => d.sortingNumber)
                .ToList();
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        
        
        public void PlayGroup()
        {
            // Auto-refresh if enabled
            if (autoPopulateFromCategory)
            {
                RefreshDialoguesFromCategory();
            }
            
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.EnqueueDialogues(dialogues);
            }
        }
        

        public string GetSequencePreview()
        {
            if (dialogues.Count == 0)
                return "No dialogues in sequence";
            
            System.Text.StringBuilder preview = new System.Text.StringBuilder();
            preview.AppendLine($"=== {groupName} Sequence ({dialogues.Count} dialogues) ===\n");
            
            for (int i = 0; i < dialogues.Count; i++)
            {
                var dialog = dialogues[i];
                if (dialog == null)
                {
                    preview.AppendLine($"{i + 1}. [Missing Dialog]");
                    continue;
                }
                
                preview.AppendLine($"{i + 1}. {dialog.title}");
                if (!string.IsNullOrEmpty(dialog.characterName))
                {
                    preview.AppendLine($"   Speaker: {dialog.characterName}");
                }
                preview.AppendLine($"   Text: \"{dialog.dialogText}\"");
                preview.AppendLine($"   Duration: {dialog.displayDuration}s\n");
            }
            
            return preview.ToString();
        }
        

        public List<string> GetDialogueTitles()
        {
            List<string> titles = new List<string>();
            foreach (var dialog in dialogues)
            {
                if (dialog != null)
                {
                    titles.Add(dialog.title);
                }
            }
            return titles;
        }
    }
}
