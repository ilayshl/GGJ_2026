using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts.Editor
{
    [System.Serializable]
    public class DialogItem
    {
        public string title = "New Dialog";
        public int sortingNumber = 0;
        public string dialogText = "";
        public string characterName = "";
        public float displayDuration = 3f;
        public bool isImportant = false;
    }

    public class DialogEditor : EditorWindow
    {
        private List<DialogItem> dialogItems = new List<DialogItem>();
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

            // Add button
            var addButton = new Button(() => AddNewDialog()) { text = "Add Dialog" };
            addButton.style.marginBottom = 10;
            leftPane.Add(addButton);

            // Dialog list
            dialogListView = new ListView();
            dialogListView.style.flexGrow = 1;
            dialogListView.selectionType = SelectionType.Single;
            dialogListView.onSelectionChange += OnDialogSelected;
            leftPane.Add(dialogListView);

            // Remove button
            var removeButton = new Button(() => RemoveSelectedDialog()) { text = "Remove Selected" };
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
            if (selectedList.Length > 0 && selectedList[0] is DialogItem selectedDialog)
            {
                selectedDialogIndex = dialogItems.IndexOf(selectedDialog);
                ShowDialogEditor(selectedDialog);
            }
        }

        private void ShowDialogEditor(DialogItem dialog)
        {
            rightPane.Clear();

            var titleLabel = new Label("Dialog Properties");
            titleLabel.style.fontSize = 16;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 20;
            rightPane.Add(titleLabel);

            // Title field
            var titleField = new TextField("Title:");
            titleField.value = dialog.title;
            titleField.RegisterValueChangedCallback(evt =>
            {
                dialog.title = evt.newValue;
                RefreshDialogList();
            });
            rightPane.Add(titleField);

            // Sorting number field
            var sortingField = new IntegerField("Sorting Number:");
            sortingField.value = dialog.sortingNumber;
            sortingField.RegisterValueChangedCallback(evt =>
            {
                dialog.sortingNumber = evt.newValue;
                RefreshDialogList();
            });
            rightPane.Add(sortingField);

            // Character name field
            var characterField = new TextField("Character Name:");
            characterField.value = dialog.characterName;
            characterField.RegisterValueChangedCallback(evt => dialog.characterName = evt.newValue);
            rightPane.Add(characterField);

            // Dialog text field (multiline)
            var textField = new TextField("Dialog Text:");
            textField.multiline = true;
            textField.style.height = 100;
            textField.value = dialog.dialogText;
            textField.RegisterValueChangedCallback(evt => dialog.dialogText = evt.newValue);
            rightPane.Add(textField);

            // Display duration field
            var durationField = new FloatField("Display Duration (seconds):");
            durationField.value = dialog.displayDuration;
            durationField.RegisterValueChangedCallback(evt => dialog.displayDuration = evt.newValue);
            rightPane.Add(durationField);

            // Important toggle
            var importantToggle = new Toggle("Important Dialog:");
            importantToggle.value = dialog.isImportant;
            importantToggle.RegisterValueChangedCallback(evt => dialog.isImportant = evt.newValue);
            rightPane.Add(importantToggle);

            // Add some spacing
            var spacer = new VisualElement();
            spacer.style.height = 20;
            rightPane.Add(spacer);

            // Save button
            var saveButton = new Button(() => SaveDialogs()) { text = "Save All Dialogs" };
            rightPane.Add(saveButton);
        }

        private void AddNewDialog()
        {
            var newDialog = new DialogItem();
            newDialog.sortingNumber = dialogItems.Count;
            dialogItems.Add(newDialog);
            RefreshDialogList();
        }

        private void RemoveSelectedDialog()
        {
            if (selectedDialogIndex >= 0 && selectedDialogIndex < dialogItems.Count)
            {
                dialogItems.RemoveAt(selectedDialogIndex);
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

                RefreshDialogList();
            }
        }


        private void SaveDialogs()
        {
            //TODO save to ScriptableObject, JSON, or any other format you prefer
            Debug.Log("Dialogs saved! (Implement your saving logic here)");
        }
    }
}