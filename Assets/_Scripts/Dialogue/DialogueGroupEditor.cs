using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor
{
    [CustomEditor(typeof(DialogueGroup))]
    public class DialogueGroupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DialogueGroup group = (DialogueGroup)target;
            
            DrawDefaultInspector();
            
            EditorGUILayout.Space(10);
            
            // Preview button
            if (GUILayout.Button("📋 Preview Sequence", GUILayout.Height(30)))
            {
                ShowSequencePreview(group);
            }
            
            EditorGUILayout.Space(5);
            
            // Refresh button (only show if auto-populate is enabled)
            if (group.autoPopulateFromCategory)
            {
                if (GUILayout.Button("🔄 Refresh from Category", GUILayout.Height(25)))
                {
                    group.RefreshDialoguesFromCategory();
                    EditorUtility.DisplayDialog("Refreshed", 
                        $"Found {group.dialogues.Count} dialogue(s) in category '{group.groupName}'", 
                        "OK");
                }
            }
            
            EditorGUILayout.Space(5);
            
            // Play in editor button (only works in play mode)
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("▶️ Play Sequence (Play Mode Only)", GUILayout.Height(25)))
            {
                group.PlayGroup();
            }
            GUI.enabled = true;
            
            // Show stats
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                $"Total Dialogues: {group.dialogues.Count}\n" +
                $"Total Duration: {CalculateTotalDuration(group)}s\n" +
                $"Mode: {(group.autoPopulateFromCategory ? "Auto-populate" : "Manual")}",
                MessageType.Info
            );
        }
        
        private void ShowSequencePreview(DialogueGroup group)
        {
            string preview = group.GetSequencePreview();
            
            // Create a scrollable text area window
            DialogueSequencePreviewWindow.ShowWindow(group.groupName, preview);
        }
        
        private float CalculateTotalDuration(DialogueGroup group)
        {
            float total = 0f;
            foreach (var dialog in group.dialogues)
            {
                if (dialog != null)
                {
                    total += dialog.displayDuration;
                }
            }
            return total;
        }
    }
    
    public class DialogueSequencePreviewWindow : EditorWindow
    {
        private string _title;
        private string _content;
        private Vector2 _scrollPosition;
        
        public static void ShowWindow(string title, string content)
        {
            DialogueSequencePreviewWindow window = GetWindow<DialogueSequencePreviewWindow>(true, "Dialogue Sequence Preview");
            window._title = title;
            window._content = content;
            window.minSize = new Vector2(500, 400);
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField(_title, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            EditorGUILayout.TextArea(_content, GUILayout.ExpandHeight(true));
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("Close", GUILayout.Height(30)))
            {
                Close();
            }
        }
    }
}
