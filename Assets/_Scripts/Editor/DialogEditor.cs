using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts.Editor
{
    public class DialogEditor : EditorWindow
    {
        private List<DialogData> dialogItems = new List<DialogData>();
        private List<DialogueGroup> dialogueGroups = new List<DialogueGroup>();
        private Dictionary<string, List<DialogData>> groupedDialogs = new Dictionary<string, List<DialogData>>();
        
        private ListView dialogListView;
        private VisualElement rightPane;
        private int selectedDialogIndex = -1;
        
        private string currentFilterGroup = "All";
        private DropdownField groupFilterDropdown;
        
        // Preset folder for all dialogue assets
        private const string PRESET_FOLDER = "Assets/Resources/Dialogues";

        [MenuItem("Matan's Scripts/Dialog Editor")]
        public static void ShowDialogEditor()
        {
            DialogEditor wnd = GetWindow<DialogEditor>();
            wnd.titleContent = new GUIContent("Dialog Editor");
            wnd.minSize = new Vector2(800, 600);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // Create split view
            var splitWindow = new TwoPaneSplitView(0, 350, TwoPaneSplitViewOrientation.Horizontal);
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

            EnsurePresetFolderExists();
            LoadAllDialogs();
            LoadAllGroups();
            RefreshDialogList();
        }

        private void EnsurePresetFolderExists()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder(PRESET_FOLDER))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");
            }
        }

        private void SetupLeftPane(VisualElement leftPane)
        {
            leftPane.style.paddingTop = 10;
            leftPane.style.paddingLeft = 10;
            leftPane.style.paddingRight = 10;
            leftPane.style.paddingBottom = 10;

            // Title
            var titleLabel = new Label("Dialogue Editor");
            titleLabel.style.fontSize = 18;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 10;
            leftPane.Add(titleLabel);

            // Group filter dropdown
            var filterContainer = new VisualElement();
            filterContainer.style.flexDirection = FlexDirection.Row;
            filterContainer.style.marginBottom = 10;
            
            var filterLabel = new Label("Filter by Group:");
            filterLabel.style.marginRight = 5;
            filterLabel.style.alignSelf = Align.Center;
            filterContainer.Add(filterLabel);
            
            groupFilterDropdown = new DropdownField();
            groupFilterDropdown.style.flexGrow = 1;
            groupFilterDropdown.RegisterValueChangedCallback(evt =>
            {
                currentFilterGroup = evt.newValue;
                RefreshDialogList();
            });
            filterContainer.Add(groupFilterDropdown);
            
            leftPane.Add(filterContainer);

            // Stats label
            var statsLabel = new Label();
            statsLabel.name = "stats-label";
            statsLabel.style.fontSize = 11;
            statsLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
            statsLabel.style.marginBottom = 10;
            leftPane.Add(statsLabel);

            // Buttons row
            var buttonRow = new VisualElement();
            buttonRow.style.flexDirection = FlexDirection.Row;
            buttonRow.style.marginBottom = 5;
            
            var refreshButton = new Button(() => { LoadAllDialogs(); LoadAllGroups(); RefreshDialogList(); }) 
            { text = "🔄 Refresh" };
            refreshButton.style.flexGrow = 1;
            buttonRow.Add(refreshButton);
            
            var validateButton = new Button(ValidateAllDialogues) 
            { text = "✓ Validate" };
            validateButton.style.flexGrow = 1;
            buttonRow.Add(validateButton);
            
            leftPane.Add(buttonRow);

            // Create Dialog button
            var createDialogButton = new Button(CreateNewDialog) { text = "+ Create New Dialog" };
            createDialogButton.style.marginBottom = 5;
            leftPane.Add(createDialogButton);
            
            // Separator
            var separator = new VisualElement();
            separator.style.height = 1;
            separator.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            separator.style.marginTop = 5;
            separator.style.marginBottom = 10;
            leftPane.Add(separator);
            
            // Group Assets Section
            var groupAssetsLabel = new Label("Dialogue Group Assets (Auto-Synced)");
            groupAssetsLabel.style.fontSize = 14;
            groupAssetsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            groupAssetsLabel.style.marginBottom = 5;
            leftPane.Add(groupAssetsLabel);
            
            var groupAssetsHint = new Label("Automatically created from dialogue categories");
            groupAssetsHint.style.fontSize = 10;
            groupAssetsHint.style.color = Color.gray;
            groupAssetsHint.style.marginBottom = 5;
            leftPane.Add(groupAssetsHint);
            
            // Sync all button
            var syncAllButton = new Button(SyncAllDialogueGroups) 
            { 
                text = "🔄 Sync All Groups",
                tooltip = "Manually sync all dialogue groups"
            };
            syncAllButton.style.marginBottom = 5;
            leftPane.Add(syncAllButton);
            
            // Group assets list
            var groupAssetsListView = new ListView();
            groupAssetsListView.name = "group-assets-list";
            groupAssetsListView.style.height = 150;
            groupAssetsListView.style.marginBottom = 10;
            groupAssetsListView.selectionType = SelectionType.Single;
            groupAssetsListView.selectionChanged += (items) =>
            {
                var selected = items.FirstOrDefault() as DialogueGroup;
                if (selected != null)
                {
                    Selection.activeObject = selected;
                    EditorGUIUtility.PingObject(selected);
                }
            };
            leftPane.Add(groupAssetsListView);
            
            // Separator before dialog list
            var separator2 = new VisualElement();
            separator2.style.height = 1;
            separator2.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            separator2.style.marginBottom = 10;
            leftPane.Add(separator2);
            
            // Dialogues Section Header
            var dialogsLabel = new Label("Dialogues");
            dialogsLabel.style.fontSize = 14;
            dialogsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            dialogsLabel.style.marginBottom = 5;
            leftPane.Add(dialogsLabel);
            
            // Dialog list
            dialogListView = new ListView();
            dialogListView.style.flexGrow = 1;
            dialogListView.selectionType = SelectionType.Single;
            dialogListView.selectionChanged += OnDialogSelected;
            leftPane.Add(dialogListView);

            // Remove button
            var removeButton = new Button(DeleteSelectedDialog) { text = "Delete Selected Dialog" };
            removeButton.style.marginTop = 10;
            removeButton.style.backgroundColor = new Color(0.8f, 0.3f, 0.3f);
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
            groupedDialogs.Clear();
            
            string[] guids = AssetDatabase.FindAssets("t:DialogData");
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DialogData dialog = AssetDatabase.LoadAssetAtPath<DialogData>(path);
                if (dialog != null)
                {
                    dialogItems.Add(dialog);
                    
                    // Group dialogues
                    string group = string.IsNullOrEmpty(dialog.groupName) ? "Default" : dialog.groupName;
                    if (!groupedDialogs.ContainsKey(group))
                    {
                        groupedDialogs[group] = new List<DialogData>();
                    }
                    groupedDialogs[group].Add(dialog);
                }
            }

            Debug.Log($"Loaded {dialogItems.Count} dialog(s) from project across {groupedDialogs.Count} group(s).");
        }

        private void LoadAllGroups()
        {
            dialogueGroups.Clear();
            string[] guids = AssetDatabase.FindAssets("t:DialogueGroup");
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DialogueGroup group = AssetDatabase.LoadAssetAtPath<DialogueGroup>(path);
                if (group != null)
                {
                    dialogueGroups.Add(group);
                }
            }
            
            Debug.Log($"Loaded {dialogueGroups.Count} dialogue group(s).");
        }

        private void RefreshDialogList()
        {
            // Update group filter dropdown
            List<string> groupNames = new List<string> { "All" };
            groupNames.AddRange(groupedDialogs.Keys.OrderBy(k => k));
            groupFilterDropdown.choices = groupNames;
            
            if (!groupNames.Contains(currentFilterGroup))
            {
                currentFilterGroup = "All";
            }
            groupFilterDropdown.value = currentFilterGroup;

            // Filter dialogues based on selected group
            List<DialogData> filteredDialogs;
            if (currentFilterGroup == "All")
            {
                filteredDialogs = dialogItems;
            }
            else
            {
                filteredDialogs = groupedDialogs.ContainsKey(currentFilterGroup) 
                    ? groupedDialogs[currentFilterGroup] 
                    : new List<DialogData>();
            }

            // Sort dialogs by sorting number
            filteredDialogs.Sort((a, b) => a.sortingNumber.CompareTo(b.sortingNumber));

            // Update stats
            var statsLabel = rootVisualElement.Q<Label>("stats-label");
            if (statsLabel != null)
            {
                statsLabel.text = $"Showing {filteredDialogs.Count} of {dialogItems.Count} dialogues | {groupedDialogs.Count} groups | {dialogueGroups.Count} group assets";
            }

            dialogListView.itemsSource = filteredDialogs;
            dialogListView.makeItem = () =>
            {
                var container = new VisualElement();
                container.style.flexDirection = FlexDirection.Column;
                container.style.paddingTop = 5;
                container.style.paddingBottom = 5;
                container.style.paddingLeft = 5;
                container.style.paddingRight = 5;
                container.style.borderBottomWidth = 1;
                container.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);

                var topRow = new VisualElement();
                topRow.style.flexDirection = FlexDirection.Row;
                topRow.style.justifyContent = Justify.SpaceBetween;

                var titleLabel = new Label();
                titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                titleLabel.style.flexGrow = 1;
                topRow.Add(titleLabel);

                var sortingLabel = new Label();
                sortingLabel.style.color = Color.gray;
                sortingLabel.style.fontSize = 12;
                topRow.Add(sortingLabel);

                container.Add(topRow);

                var bottomRow = new VisualElement();
                bottomRow.style.flexDirection = FlexDirection.Row;
                bottomRow.style.marginTop = 2;

                var groupLabel = new Label();
                groupLabel.style.fontSize = 11;
                groupLabel.style.color = new Color(0.5f, 0.8f, 1f);
                bottomRow.Add(groupLabel);

                var characterLabel = new Label();
                characterLabel.style.fontSize = 11;
                characterLabel.style.color = new Color(0.8f, 0.8f, 0.5f);
                characterLabel.style.marginLeft = 10;
                bottomRow.Add(characterLabel);

                container.Add(bottomRow);

                return container;
            };

            dialogListView.bindItem = (element, index) =>
            {
                if (index < filteredDialogs.Count)
                {
                    var dialog = filteredDialogs[index];
                    
                    var topRow = element.Children().ElementAt(0);
                    var titleLabel = topRow.Children().ElementAt(0) as Label;
                    var sortingLabel = topRow.Children().ElementAt(1) as Label;
                    
                    var bottomRow = element.Children().ElementAt(1);
                    var groupLabel = bottomRow.Children().ElementAt(0) as Label;
                    var characterLabel = bottomRow.Children().ElementAt(1) as Label;

                    titleLabel.text = dialog.title;
                    sortingLabel.text = $"#{dialog.sortingNumber}";
                    groupLabel.text = $"📁 {dialog.groupName}";
                    characterLabel.text = string.IsNullOrEmpty(dialog.characterName) 
                        ? "👤 (no character)" 
                        : $"👤 {dialog.characterName}";
                }
            };

            dialogListView.Rebuild();
            
            // Update group assets list
            var groupAssetsListView = rootVisualElement.Q<ListView>("group-assets-list");
            if (groupAssetsListView != null)
            {
                groupAssetsListView.itemsSource = dialogueGroups;
                groupAssetsListView.makeItem = () =>
                {
                    var container = new VisualElement();
                    container.style.flexDirection = FlexDirection.Column;
                    container.style.paddingTop = 3;
                    container.style.paddingBottom = 3;
                    container.style.paddingLeft = 3;
                    container.style.paddingRight = 3;
                    container.style.backgroundColor = new Color(0.2f, 0.2f, 0.25f);
                    container.style.marginBottom = 2;
                    container.style.borderBottomLeftRadius = 3;
                    container.style.borderBottomRightRadius = 3;
                    container.style.borderTopLeftRadius = 3;
                    container.style.borderTopRightRadius = 3;
                    
                    var topRow = new VisualElement();
                    topRow.style.flexDirection = FlexDirection.Row;
                    
                    var icon = new Label("📦");
                    icon.style.marginRight = 5;
                    topRow.Add(icon);
                    
                    var nameLabel = new Label();
                    nameLabel.style.flexGrow = 1;
                    nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                    topRow.Add(nameLabel);
                    
                    var countLabel = new Label();
                    countLabel.style.fontSize = 10;
                    countLabel.style.color = Color.gray;
                    topRow.Add(countLabel);
                    
                    container.Add(topRow);
                    
                    var bottomRow = new VisualElement();
                    bottomRow.style.flexDirection = FlexDirection.Row;
                    bottomRow.style.marginTop = 2;
                    
                    var statusLabel = new Label();
                    statusLabel.style.fontSize = 9;
                    statusLabel.style.color = new Color(0.5f, 1f, 0.5f);
                    bottomRow.Add(statusLabel);
                    
                    container.Add(bottomRow);
                    return container;
                };
                
                groupAssetsListView.bindItem = (element, index) =>
                {
                    if (index < dialogueGroups.Count)
                    {
                        var group = dialogueGroups[index];
                        
                        var topRow = element.Children().ElementAt(0);
                        var nameLabel = topRow.Children().ElementAt(1) as Label;
                        var countLabel = topRow.Children().ElementAt(2) as Label;
                        
                        var bottomRow = element.Children().ElementAt(1);
                        var statusLabel = bottomRow.Children().ElementAt(0) as Label;
                        
                        nameLabel.text = group.groupName;
                        countLabel.text = $"({group.dialogues.Count})";
                        statusLabel.text = group.autoPopulateFromCategory ? "✓ Auto-synced" : "⚠ Manual mode";
                    }
                };
                
                groupAssetsListView.Rebuild();
            }
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

            var scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;

            var titleLabel = new Label($"Editing: {dialog.title}");
            titleLabel.style.fontSize = 16;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.marginBottom = 10;
            scrollView.Add(titleLabel);

            // Show asset path
            string assetPath = AssetDatabase.GetAssetPath(dialog);
            var pathLabel = new Label($"📄 {assetPath}");
            pathLabel.style.fontSize = 10;
            pathLabel.style.color = Color.gray;
            pathLabel.style.marginBottom = 15;
            scrollView.Add(pathLabel);

            // === BASIC INFO ===
            var basicFoldout = new Foldout { text = "📋 Basic Info", value = true };
            
            // Title field with validation
            var titleField = new TextField("Title (Unique):");
            titleField.value = dialog.title;
            titleField.RegisterValueChangedCallback(evt =>
            {
                string sanitized = SanitizeTitle(evt.newValue);
                if (IsTitleUnique(sanitized, dialog))
                {
                    Undo.RecordObject(dialog, "Change Dialog Title");
                    dialog.title = sanitized;
                    
                    // Rename asset file to match title
                    RenameAssetToMatchTitle(dialog);
                    
                    EditorUtility.SetDirty(dialog);
                    RefreshDialogList();
                    titleField.style.backgroundColor = Color.clear;
                }
                else
                {
                    titleField.style.backgroundColor = new Color(1f, 0.5f, 0.5f, 0.3f);
                    Debug.LogWarning($"Title '{sanitized}' is not unique!");
                }
            });
            basicFoldout.Add(titleField);

            // Group dropdown with manual entry option
            var groupContainer = new VisualElement();
            groupContainer.style.flexDirection = FlexDirection.Row;
            
            // Get all unique group names
            List<string> allGroupNames = new List<string> { "Default" };
            allGroupNames.AddRange(groupedDialogs.Keys.Where(k => k != "Default").OrderBy(k => k));
            
            var groupDropdown = new DropdownField("Group Name:", allGroupNames, dialog.groupName);
            groupDropdown.style.flexGrow = 1;
            groupDropdown.RegisterValueChangedCallback(evt =>
            {
                string oldGroup = dialog.groupName;
                
                Undo.RecordObject(dialog, "Change Dialog Group");
                dialog.groupName = evt.newValue;
                EditorUtility.SetDirty(dialog);
                
                // Sync both old and new groups
                SyncDialogueGroup(oldGroup);
                SyncDialogueGroup(evt.newValue);
                
                LoadAllDialogs();
                LoadAllGroups();
                RefreshDialogList();
            });
            basicFoldout.Add(groupDropdown);
            
            // Custom group name entry
            var groupRow = new VisualElement();
            groupRow.style.flexDirection = FlexDirection.Row;
            groupRow.style.marginTop = 5;
            
            var customGroupField = new TextField();
            customGroupField.style.flexGrow = 1;
            customGroupField.style.marginLeft = 5;
            customGroupField.value = "";
            customGroupField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    if (!string.IsNullOrWhiteSpace(customGroupField.value))
                    {
                        string previousGroup = dialog.groupName;
                        
                        Undo.RecordObject(dialog, "Change Dialog Group");
                        dialog.groupName = customGroupField.value.Trim();
                        EditorUtility.SetDirty(dialog);
                        customGroupField.value = "";
                        
                        // Sync both old and new groups
                        SyncDialogueGroup(previousGroup);
                        SyncDialogueGroup(dialog.groupName);
                        
                        // Reload and refresh
                        LoadAllDialogs();
                        LoadAllGroups();
                        RefreshDialogList();
                        ShowDialogEditor(dialog);
                    }
                }
            });
            
            var applyButton = new Button(() =>
            {
                if (!string.IsNullOrWhiteSpace(customGroupField.value))
                {
                    string previousGroup = dialog.groupName;
                    
                    Undo.RecordObject(dialog, "Change Dialog Group");
                    dialog.groupName = customGroupField.value.Trim();
                    EditorUtility.SetDirty(dialog);
                    customGroupField.value = "";
                    
                    // Sync both old and new groups
                    SyncDialogueGroup(previousGroup);
                    SyncDialogueGroup(dialog.groupName);
                    
                    // Reload and refresh
                    LoadAllDialogs();
                    LoadAllGroups();
                    RefreshDialogList();
                    ShowDialogEditor(dialog);
                }
            })
            { text = "Apply" };
            applyButton.style.width = 60;
            applyButton.style.marginLeft = 5;
            
            groupRow.Add(customGroupField);
            groupRow.Add(applyButton);
            basicFoldout.Add(groupRow);
            
            var groupHint = new Label("Type a new group name above or select from dropdown");
            groupHint.style.fontSize = 9;
            groupHint.style.color = Color.gray;
            groupHint.style.marginTop = 2;
            groupHint.style.marginBottom = 10;
            basicFoldout.Add(groupHint);

            var sortingField = new IntegerField("Sorting Number:");
            sortingField.value = dialog.sortingNumber;
            sortingField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(dialog, "Change Sorting Number");
                dialog.sortingNumber = evt.newValue;
                EditorUtility.SetDirty(dialog);
                
                // Re-sync the group since sorting changed
                SyncDialogueGroup(dialog.groupName);
                LoadAllGroups();
                RefreshDialogList();
            });
            basicFoldout.Add(sortingField);

            var characterField = new TextField("Character Name:");
            characterField.value = dialog.characterName;
            characterField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(dialog, "Change Character Name");
                dialog.characterName = evt.newValue;
                EditorUtility.SetDirty(dialog);
                RefreshDialogList();
            });
            basicFoldout.Add(characterField);
            
            scrollView.Add(basicFoldout);

            // === CONTENT ===
            var contentFoldout = new Foldout { text = "💬 Content", value = true };
            
            var textField = new TextField("Dialog Text:");
            textField.multiline = true;
            textField.style.height = 120;
            textField.value = dialog.dialogText;
            textField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(dialog, "Change Dialog Text");
                dialog.dialogText = evt.newValue;
                EditorUtility.SetDirty(dialog);
            });
            contentFoldout.Add(textField);
            
            var charCountLabel = new Label($"Character count: {dialog.dialogText.Length}");
            charCountLabel.style.fontSize = 10;
            charCountLabel.style.color = Color.gray;
            contentFoldout.Add(charCountLabel);
            
            scrollView.Add(contentFoldout);

            // === TIMING ===
            var timingFoldout = new Foldout { text = "⏱️ Timing", value = false };
            
            var durationField = new FloatField("Display Duration (seconds):");
            durationField.value = dialog.displayDuration;
            durationField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(dialog, "Change Display Duration");
                dialog.displayDuration = evt.newValue;
                EditorUtility.SetDirty(dialog);
            });
            timingFoldout.Add(durationField);
            
            var typingSpeedField = new FloatField("Typing Speed Override (-1 = default):");
            typingSpeedField.value = dialog.typingSpeedOverride;
            typingSpeedField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(dialog, "Change Typing Speed");
                dialog.typingSpeedOverride = evt.newValue;
                EditorUtility.SetDirty(dialog);
            });
            timingFoldout.Add(typingSpeedField);
            
            scrollView.Add(timingFoldout);

            // === BUTTONS ===
            var spacer = new VisualElement();
            spacer.style.height = 20;
            scrollView.Add(spacer);

            var buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;
            
            var saveButton = new Button(() => SaveDialog(dialog)) { text = "💾 Save" };
            saveButton.style.flexGrow = 1;
            saveButton.style.backgroundColor = new Color(0.3f, 0.7f, 0.3f);
            buttonContainer.Add(saveButton);

            var selectButton = new Button(() => Selection.activeObject = dialog) { text = "🔍 Select in Project" };
            selectButton.style.flexGrow = 1;
            buttonContainer.Add(selectButton);
            
            scrollView.Add(buttonContainer);
            
            rightPane.Add(scrollView);
        }
        

        private void CreateNewDialog()
        {
            // Ensure the Resources/Dialogues folder exists
            string dialoguesFolder = "Assets/Resources/Dialogues";
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder(dialoguesFolder))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");
            }
            
            // Let user choose the filename
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Dialog",
                "NewDialog",
                "asset",
                "Enter a name for the new dialog",
                dialoguesFolder
            );
            
            if (!string.IsNullOrEmpty(path))
            {
                string filename = Path.GetFileNameWithoutExtension(path);
                string sanitizedTitle = SanitizeTitle(filename);
                
                // Check if title is unique
                if (!IsTitleUnique(sanitizedTitle, null))
                {
                    sanitizedTitle = GetUniqueTitle(sanitizedTitle);
                    EditorUtility.DisplayDialog(
                        "Title Conflict", 
                        $"A dialog with that title already exists.\nUsing title: '{sanitizedTitle}' instead.", 
                        "OK"
                    );
                }
                
                DialogData newDialog = ScriptableObject.CreateInstance<DialogData>();
                newDialog.title = sanitizedTitle;
                newDialog.groupName = currentFilterGroup != "All" ? currentFilterGroup : "Default";
                newDialog.sortingNumber = dialogItems.Count;
                
                AssetDatabase.CreateAsset(newDialog, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                LoadAllDialogs();
                RefreshDialogList();
                
                Debug.Log($"Created new dialog: {path}");
                
                // Select the newly created dialog
                Selection.activeObject = newDialog;
            }
        }
        

        private void DeleteSelectedDialog()
        {
            if (selectedDialogIndex >= 0 && selectedDialogIndex < dialogItems.Count)
            {
                var dialog = dialogItems[selectedDialogIndex];
                string groupToSync = dialog.groupName;
                
                if (EditorUtility.DisplayDialog(
                    "Delete Dialog",
                    $"Are you sure you want to delete '{dialog.title}'?\nThis action cannot be undone.",
                    "Delete", "Cancel"))
                {
                    string path = AssetDatabase.GetAssetPath(dialog);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    
                    selectedDialogIndex = -1;
                    SetupRightPane();
                    LoadAllDialogs();
                    
                    // Sync the group this dialogue belonged to
                    SyncDialogueGroup(groupToSync);
                    
                    LoadAllGroups();
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
            
            // Automatically sync the dialogue group for this category
            SyncDialogueGroup(dialog.groupName);
            
            // Reload to reflect changes
            LoadAllGroups();
            RefreshDialogList();
            
            Debug.Log($"Saved dialog: {dialog.name}");
        }
        
        private void SyncDialogueGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                groupName = "Default";
            }
            
            // Find all dialogues in this group
            List<DialogData> groupDialogues = dialogItems
                .Where(d => d.groupName == groupName)
                .OrderBy(d => d.sortingNumber)
                .ToList();
            
            if (groupDialogues.Count == 0)
            {
                Debug.LogWarning($"No dialogues found for group '{groupName}'");
                return;
            }
            
            // Check if group asset already exists
            string[] existingGroupGuids = AssetDatabase.FindAssets($"t:DialogueGroup");
            DialogueGroup existingGroup = null;
            
            foreach (string guid in existingGroupGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DialogueGroup group = AssetDatabase.LoadAssetAtPath<DialogueGroup>(path);
                
                if (group != null && group.groupName == groupName && group.autoPopulateFromCategory)
                {
                    existingGroup = group;
                    break;
                }
            }
            
            if (existingGroup != null)
            {
                // Update existing group
                existingGroup.dialogues = groupDialogues;
                EditorUtility.SetDirty(existingGroup);
                Debug.Log($"Updated DialogueGroup '{groupName}' with {groupDialogues.Count} dialogue(s)");
            }
            else
            {
                // Create new group
                EnsurePresetFolderExists();
                
                DialogueGroup newGroup = ScriptableObject.CreateInstance<DialogueGroup>();
                newGroup.groupName = groupName;
                newGroup.autoPopulateFromCategory = true;
                newGroup.dialogues = groupDialogues;
                newGroup.description = $"Auto-generated group for '{groupName}' category";
                
                string fileName = SanitizeTitle(groupName);
                string path = $"{PRESET_FOLDER}/{fileName}_Group.asset";
                path = AssetDatabase.GenerateUniqueAssetPath(path);
                
                AssetDatabase.CreateAsset(newGroup, path);
                AssetDatabase.SaveAssets();
                
                Debug.Log($"Created new DialogueGroup '{groupName}' at {path}");
            }
            
            AssetDatabase.Refresh();
        }
        
        private void SyncAllDialogueGroups()
        {
            // Get all unique group names
            HashSet<string> allGroupNames = new HashSet<string>();
            foreach (var dialog in dialogItems)
            {
                string groupName = string.IsNullOrEmpty(dialog.groupName) ? "Default" : dialog.groupName;
                allGroupNames.Add(groupName);
            }
            
            // Sync each group
            foreach (string groupName in allGroupNames)
            {
                SyncDialogueGroup(groupName);
            }
            
            // Reload everything
            LoadAllGroups();
            RefreshDialogList();
            
            EditorUtility.DisplayDialog("Sync Complete", 
                $"Synced {allGroupNames.Count} dialogue group(s)", 
                "OK");
        }

        private void ValidateAllDialogues()
        {
            List<string> issues = new List<string>();
            HashSet<string> titles = new HashSet<string>();
            
            foreach (var dialog in dialogItems)
            {
                // Check for duplicate titles
                if (titles.Contains(dialog.title))
                {
                    issues.Add($"❌ Duplicate title: '{dialog.title}'");
                }
                else
                {
                    titles.Add(dialog.title);
                }
                
                // Check if title matches filename
                string filename = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(dialog));
                if (dialog.title != filename)
                {
                    issues.Add($"⚠️ Title/filename mismatch: '{dialog.title}' vs '{filename}'");
                }
                
                // Check for empty content
                if (string.IsNullOrWhiteSpace(dialog.dialogText))
                {
                    issues.Add($"⚠️ Empty dialog text: '{dialog.title}'");
                }
            }
            
            if (issues.Count == 0)
            {
                EditorUtility.DisplayDialog("Validation Complete", "✓ All dialogues are valid!", "OK");
            }
            else
            {
                string message = $"Found {issues.Count} issue(s):\n\n" + string.Join("\n", issues);
                EditorUtility.DisplayDialog("Validation Issues", message, "OK");
                Debug.LogWarning(message);
            }
        }

        private string SanitizeTitle(string title)
        {
            // Remove invalid filename characters
            string invalid = new string(Path.GetInvalidFileNameChars());
            Regex regex = new Regex($"[{Regex.Escape(invalid)}]");
            return regex.Replace(title.Trim(), "_");
        }

        private bool IsTitleUnique(string title, DialogData currentDialog)
        {
            foreach (var dialog in dialogItems)
            {
                if (dialog != currentDialog && dialog.title == title)
                {
                    return false;
                }
            }
            return true;
        }

        private string GetUniqueTitle(string baseTitle)
        {
            string title = baseTitle;
            int counter = 1;
            
            while (!IsTitleUnique(title, null))
            {
                title = $"{baseTitle}_{counter}";
                counter++;
            }
            
            return title;
        }

        private void RenameAssetToMatchTitle(DialogData dialog)
        {
            string currentPath = AssetDatabase.GetAssetPath(dialog);
            string directory = Path.GetDirectoryName(currentPath);
            string newPath = Path.Combine(directory, $"{dialog.title}.asset");
            
            if (currentPath != newPath)
            {
                string error = AssetDatabase.RenameAsset(currentPath, $"{dialog.title}.asset");
                if (string.IsNullOrEmpty(error))
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log($"Renamed asset to: {newPath}");
                }
                else
                {
                    Debug.LogError($"Failed to rename asset:{error}");
                }
            }
        }
    }
}