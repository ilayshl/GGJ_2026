using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts.Editor
{
    public class DialogEditor : EditorWindow
    {
        private List<DialogData> dialogItems = new List<DialogData>();
        private ListView dialogListView;
        private VisualElement rightPane;
        private int selectedDialogIndex = -1;

        [MenuItem("Matan's Scripts/Dialog Editor")]
        public static void ShowDialogEditor()
        {
            DialogEditor wnd = GetWindow<DialogEditor>();
            wnd.titleContent = new GUIContent("Dialog Editor");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // Create split view
            var splitWindow = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal);
            root.Add(splitWindow);

            // Create left and right panes
            var leftPane = new VisualElement();
            rightPane = new VisualElement();
            splitWindow.Add(leftPane);
            splitWindow.Add(rightPane);

            // Setup left pane (dialog list)
            SetupLeftPane(leftPane);

            // Setup right pane (dialog editor)
            SetupRightPane();

            splitWindow.StretchToParentSize();

            LoadAllDialogs();
            RefreshDialogList();
        }

        private void SetupLeftPane(VisualElement leftPane)
        {
            leftPane.style.paddingTop = 10;
            leftPane.style.paddingLeft = 10;
            leftPane.style.paddingRight = 10;
            leftPane.style.paddingBottom = 10;

            // Title
            var titleLabel = new Label("Dialogs");
            titleLabel.style.fontSize = 16;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 10;
            leftPane.Add(titleLabel);

            // Refresh button
            var refreshButton = new Button(() => { LoadAllDialogs(); RefreshDialogList(); }) { text = "Refresh List" };
            refreshButton.style.marginBottom = 5;
            leftPane.Add(refreshButton);

            // Add button
            var addButton = new Button(() => CreateNewDialog()) { text = "Create New Dialog" };
            addButton.style.marginBottom = 10;
            leftPane.Add(addButton);

            // Dialog list
            dialogListView = new ListView();
            dialogListView.style.flexGrow = 1;
            dialogListView.selectionType = SelectionType.Single;
            dialogListView.onSelectionChange += OnDialogSelected;
            leftPane.Add(dialogListView);

            // Remove button
            var removeButton = new Button(() => DeleteSelectedDialog()) { text = "Delete Selected" };
            removeButton.style.marginTop = 10;
            leftPane.Add(removeButton);
        }

        private void SetupRightPane()
        {
            rightPane.style.paddingTop = 10;
            rightPane.style.paddingLeft = 10;
            rightPane.style.paddingRight = 10;
            rightPane.style.paddingBottom = 10;

            var titleLabel = new Label("Dialog Properties");
            titleLabel.style.fontSize = 16;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 10;
            rightPane.Add(titleLabel);

            var noSelectionLabel = new Label("Select a dialog from the list to edit its properties.");
            noSelectionLabel.name = "no-selection-label";
            rightPane.Add(noSelectionLabel);
        }

        private void LoadAllDialogs()
        {
            dialogItems.Clear();
            string[] guids = AssetDatabase.FindAssets("t:DialogData");
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DialogData dialog = AssetDatabase.LoadAssetAtPath<DialogData>(path);
                if (dialog != null)
                {
                    dialogItems.Add(dialog);
                }
            }

            Debug.Log($"Loaded {dialogItems.Count} dialog(s) from project.");
        }

        private void RefreshDialogList()
        {
            // Sort dialogs by sorting number
            dialogItems.Sort((a, b) => a.sortingNumber.CompareTo(b.sortingNumber));

            dialogListView.itemsSource = dialogItems;
            dialogListView.makeItem = () =>
            {
                var container = new VisualElement();
                container.style.flexDirection = FlexDirection.Row;
                container.style.justifyContent = Justify.SpaceBetween;
                container.style.paddingTop = 5;
                container.style.paddingBottom = 5;

                var titleLabel = new Label();
                titleLabel.style.flexGrow = 1;
                container.Add(titleLabel);

                var sortingLabel = new Label();
                sortingLabel.style.color = Color.gray;
                sortingLabel.style.fontSize = 12;
                container.Add(sortingLabel);

                return container;
            };

            dialogListView.bindItem = (element, index) =>
            {
                var container = element;
                var titleLabel = container.Q<Label>();
                var sortingLabel = container.Children().ElementAt(1) as Label;

                if (index < dialogItems.Count)
                {
                    var dialog = dialogItems[index];
                    titleLabel.text = dialog.title;
                    sortingLabel.text = $"#{dialog.sortingNumber}";
                }
            };

            dialogListView.Rebuild();
        }

        private void OnDialogSelected(IEnumerable<object> selectedItems)
        {
            var selectedList = selectedItems.ToArray();
            if (selectedList.Length > 0 && selectedList[0] is DialogData selectedDialog)
            {
                selectedDialogIndex = dialogItems.IndexOf(selectedDialog);
                ShowDialogEditor(selectedDialog);
            }
        }

        private void ShowDialogEditor(DialogData dialog)
        {
            rightPane.Clear();

            var titleLabel = new Label("Dialog Properties");
            titleLabel.style.fontSize = 16;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 20;
            rightPane.Add(titleLabel);

            // Show asset name
            var assetNameLabel = new Label($"Asset: {dialog.name}");
            assetNameLabel.style.color = Color.green;
            assetNameLabel.style.marginBottom = 10;
            rightPane.Add(assetNameLabel);

            // Title field
            var titleField = new TextField("Title:");
            titleField.value = dialog.title;
            titleField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(dialog, "Change Dialog Title");
                dialog.title = evt.newValue;
                EditorUtility.SetDirty(dialog);
                RefreshDialogList();
            });
            rightPane.Add(titleField);

            // Sorting number field
            var sortingField = new IntegerField("Sorting Number:");
            sortingField.value = dialog.sortingNumber;
            sortingField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(dialog, "Change Sorting Number");
                dialog.sortingNumber = evt.newValue;
                EditorUtility.SetDirty(dialog);
                RefreshDialogList();
            });
            rightPane.Add(sortingField);

            // Character name field
            var characterField = new TextField("Character Name:");
            characterField.value = dialog.characterName;
            characterField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(dialog, "Change Character Name");
                dialog.characterName = evt.newValue;
                EditorUtility.SetDirty(dialog);
            });
            rightPane.Add(characterField);

            // Dialog text field (multiline)
            var textField = new TextField("Dialog Text:");
            textField.multiline = true;
            textField.style.height = 100;
            textField.value = dialog.dialogText;
            textField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(dialog, "Change Dialog Text");
                dialog.dialogText = evt.newValue;
                EditorUtility.SetDirty(dialog);
            });
            rightPane.Add(textField);

            // Display duration field
            var durationField = new FloatField("Display Duration (seconds):");
            durationField.value = dialog.displayDuration;
            durationField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(dialog, "Change Display Duration");
                dialog.displayDuration = evt.newValue;
                EditorUtility.SetDirty(dialog);
            });
            rightPane.Add(durationField);
            

            // Add some spacing
            var spacer = new VisualElement();
            spacer.style.height = 20;
            rightPane.Add(spacer);

            // Save button
            var saveButton = new Button(() => SaveDialog(dialog)) { text = "Save Changes" };
            rightPane.Add(saveButton);

            // Select in Project button
            var selectButton = new Button(() => Selection.activeObject = dialog) { text = "Select in Project" };
            rightPane.Add(selectButton);
        }

        private void CreateNewDialog()
        {
            DialogData newDialog = ScriptableObject.CreateInstance<DialogData>();
            newDialog.sortingNumber = dialogItems.Count;
            
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Dialog",
                "NewDialog",
                "asset",
                "Please enter a file name for the new dialog"
            );
            
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newDialog, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                LoadAllDialogs();
                RefreshDialogList();
                
                Debug.Log($"Created new dialog at: {path}");
            }
        }

        private void DeleteSelectedDialog()
        {
            if (selectedDialogIndex >= 0 && selectedDialogIndex < dialogItems.Count)
            {
                var dialog = dialogItems[selectedDialogIndex];
                
                if (EditorUtility.DisplayDialog(
                    "Delete Dialog",
                    $"Are you sure you want to delete '{dialog.title}'? This action cannot be undone.",
                    "Delete", "Cancel"))
                {
                    string path = AssetDatabase.GetAssetPath(dialog);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    
                    selectedDialogIndex = -1;

                    // Clear right pane
                    rightPane.Clear();
                    var titleLabel = new Label("Dialog Properties");
                    titleLabel.style.fontSize = 16;
                    titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                    titleLabel.style.marginBottom = 10;
                    rightPane.Add(titleLabel);

                    var noSelectionLabel = new Label("Select a dialog from the list to edit its properties.");
                    rightPane.Add(noSelectionLabel);

                    LoadAllDialogs();
                    RefreshDialogList();
                    
                    Debug.Log($"Deleted dialog: {path}");
                }
            }
        }

        private void SaveDialog(DialogData dialog)
        {
            EditorUtility.SetDirty(dialog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Saved dialog: {dialog.name}");
            EditorUtility.DisplayDialog("Dialog Saved", $"Successfully saved '{dialog.title}'", "OK");
        }
    }
}