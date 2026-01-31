using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Scripts;
using DG.Tweening;
using System.Linq;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    // Events
    public static event Action<string> OnDialogueEvent;
    public static event Action<DialogData> OnDialogueStart;
    public static event Action<DialogData> OnDialogueComplete;
    public static event Action OnQueueComplete;

    public static event Action OnDialogueEndGlitch;

    [Header("Dialogue Database")] [SerializeField]
    private List<DialogData> allDialogues = new List<DialogData>();

    [Header("UI References")] [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField] private CanvasGroup dialoguePanelCanvasGroup;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text characterNameText;

    [Header("Typing Settings")] [SerializeField]
    private float defaultTypingSpeed = 0.05f;

    [SerializeField] private bool canSkipTyping = true;
    [SerializeField] private float punctuationDelay = 0.2f; // Extra delay after . , ! ?

    [Header("Animation Settings")] [SerializeField]
    private float animationDuration = 0.3f;

    [Header("Audio Settings")] [SerializeField]
    private bool useDialogueAudio = true;

    [SerializeField] private float typingSoundInterval = 0.1f; // Play typing sound every X seconds

    [SerializeField] private bool autoHideAfterDuration = true;

    [Header("Queue Settings")] [SerializeField]
    private float delayBetweenDialogues = 0.3f;

    // Queue system
    private Queue<DialogData> _dialogueQueue = new Queue<DialogData>();
    private bool _isPlayingQueue = false;

    private Coroutine _currentTypingCoroutine;
    private bool _isTyping = false;
    private bool _skipRequested = false;
    private bool _waitingForInput = false;
    private float _lastTypingSoundTime = 0f;
    private DialogData _currentDialog;

    private static DialogueManager _instance;

    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<DialogueManager>();
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

    private void Start()
    {
        // Setup canvas group if not assigned
        if (dialoguePanelCanvasGroup == null && dialoguePanel != null)
        {
            dialoguePanelCanvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
            if (dialoguePanelCanvasGroup == null)
            {
                dialoguePanelCanvasGroup = dialoguePanel.AddComponent<CanvasGroup>();
            }
        }

        // Hide dialogue panel at start
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }


        // Load all DialogData from Resources if not manually assigned
        if (allDialogues.Count == 0)
        {
            LoadAllDialoguesFromResources();
        }
    }

    private void LoadAllDialoguesFromResources()
    {
        DialogData[] dialogues = Resources.LoadAll<DialogData>("Dialogues");
        allDialogues.AddRange(dialogues);
        Debug.Log($"Loaded {allDialogues.Count} dialogues from Resources");
    }

    #region Queue Management

    public void EnqueueDialogue(DialogData dialogData)
    {
        if (dialogData == null)
        {
            Debug.LogWarning("Cannot enqueue null DialogData!");
            return;
        }

        _dialogueQueue.Enqueue(dialogData);

        // If not currently playing, start the queue
        if (!_isPlayingQueue)
        {
            StartCoroutine(ProcessDialogueQueue());
        }
    }


    public void EnqueueDialogue(string dialogueTitle)
    {
        DialogData data = allDialogues.Find(d => d.title == dialogueTitle);

        if (data == null)
        {
            Debug.LogWarning($"Dialogue with title '{dialogueTitle}' not found!");
            return;
        }

        EnqueueDialogue(data);
    }


    public void EnqueueDialogue(int index)
    {
        if (index < 0 || index >= allDialogues.Count)
        {
            Debug.LogWarning($"Dialogue index {index} is out of range!");
            return;
        }

        EnqueueDialogue(allDialogues[index]);
    }


    public void EnqueueDialogues(List<DialogData> dialogues)
    {
        if (dialogues == null || dialogues.Count == 0)
        {
            Debug.LogWarning("Dialogue list is null or empty!");
            return;
        }

        foreach (var dialogue in dialogues)
        {
            if (dialogue != null)
            {
                _dialogueQueue.Enqueue(dialogue);
            }
        }

        // If not currently playing, start the queue
        if (!_isPlayingQueue)
        {
            StartCoroutine(ProcessDialogueQueue());
        }
    }


    public void ClearQueue()
    {
        // Store current dialog before clearing
        DialogData currentDialog = _currentDialog;

        // Stop current typing coroutine if running
        if (_currentTypingCoroutine != null)
        {
            StopCoroutine(_currentTypingCoroutine);
            _currentTypingCoroutine = null;
        }

        // Clear the queue
        _dialogueQueue.Clear();

        // Ensure completion event fires for current dialog
        if (currentDialog != null)
        {
            OnDialogueComplete?.Invoke(currentDialog);
        }

        // Mark queue as not playing
        _isPlayingQueue = false;

        // Fire queue complete event
        OnQueueComplete?.Invoke();

        // Hide dialogue
        HideDialogue();

        Debug.Log("Dialogue queue cleared - all events invoked");
    }


    public void SkipToNext()
    {
        if (_waitingForInput)
        {
            _waitingForInput = false;
        }
        else if (_isTyping)
        {
            _skipRequested = true;
            // Force advance after text is complete
            StartCoroutine(ForceAdvanceAfterSkip());
        }
    }

    private IEnumerator ForceAdvanceAfterSkip()
    {
        yield return new WaitUntil(() => !_isTyping);
        yield return new WaitForSeconds(0.1f);
        if (_waitingForInput)
        {
            _waitingForInput = false;
        }
    }


    public int GetQueueCount()
    {
        return _dialogueQueue.Count;
    }


    public bool HasQueuedDialogues()
    {
        return _dialogueQueue.Count > 0;
    }

    #endregion

    #region Queue Processing

    private IEnumerator ProcessDialogueQueue()
    {
        _isPlayingQueue = true;

        while (_dialogueQueue.Count > 0)
        {
            DialogData nextDialog = _dialogueQueue.Dequeue();

            // Play the dialogue
            yield return DisplayDialogueCoroutine(nextDialog);

            // Small delay between dialogues
            if (_dialogueQueue.Count > 0)
            {
                yield return new WaitForSeconds(delayBetweenDialogues);
            }
        }

        _isPlayingQueue = false;
        OnQueueComplete?.Invoke();
    }

    #endregion

    #region Display Logic

    private IEnumerator DisplayDialogueCoroutine(DialogData dialogData)
    {
        _currentDialog = dialogData;
        bool completedNaturally = false;


        OnDialogueStart?.Invoke(dialogData);

        try
        {
            // Show and animate dialogue panel
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
                yield return AnimateDialoguePanel(dialogData.animationType, true);
            }

            // Set character name
            if (characterNameText != null)
            {
                characterNameText.text = dialogData.characterName;
                characterNameText.color = dialogData.characterNameColor;
            }

            // Play voice line if available
            if (useDialogueAudio && dialogData.dialogueVoiceLine != null)
            {
                AudioManager.Play(SoundType.Dialogue, dialogData.dialogueVoiceLine);
            }

            // Clear text and prepare for typing
            if (dialogueText != null)
            {
                dialogueText.text = "";
            }

            _isTyping = true;
            _skipRequested = false;
            _lastTypingSoundTime = 0f;

            string fullText = dialogData.dialogText;
            float typingSpeed = dialogData.typingSpeedOverride > 0
                ? dialogData.typingSpeedOverride
                : defaultTypingSpeed;

            // Type out each character
            for (int i = 0; i < fullText.Length; i++)
            {
                // Check if skip was requested
                if (_skipRequested && canSkipTyping)
                {
                    dialogueText.text = fullText;
                    break;
                }

                char currentChar = fullText[i];
                dialogueText.text += currentChar;

                // Play typing sound
                if (dialogData.playTypingSound && dialogData.typingSoundEffect != null)
                {
                    if (Time.time - _lastTypingSoundTime >= typingSoundInterval && !char.IsWhiteSpace(currentChar))
                    {
                        AudioManager.Play(SoundType.SFX, dialogData.typingSoundEffect);
                        _lastTypingSoundTime = Time.time;
                    }
                }

                // Add extra delay for punctuation
                float delay = typingSpeed;
                if (currentChar == '.' || currentChar == ',' || currentChar == '!' || currentChar == '?')
                {
                    delay += punctuationDelay;
                }

                yield return new WaitForSeconds(delay);
            }

            if (dialogData.glitch)
            {
                OnDialogueEndGlitch?.Invoke();
                Debug.Log("[DialogueManager] Invoked");
            }

            _isTyping = false;


            // Wait for player input or auto-continue
            if (autoHideAfterDuration)
            {
                _waitingForInput = true;
                float timer = 0f;

                while (timer < dialogData.displayDuration && _waitingForInput)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }

                _waitingForInput = false;
            }
            else
            {
                _waitingForInput = true;
                yield return new WaitUntil(() => !_waitingForInput);
            }


            completedNaturally = true;
        }
        finally
        {
            // This block ALWAYS executes, even if coroutine is stopped

            // Clean up state
            _isTyping = false;
            _waitingForInput = false;

            OnDialogueComplete?.Invoke(dialogData);

            // Hide dialogue panel if this is the last in queue
            if (_dialogueQueue.Count == 0)
            {
                StartCoroutine(HideDialoguePanelCoroutine(dialogData.animationType));
            }

            _currentDialog = null;
        }
    }

    private IEnumerator HideDialoguePanelCoroutine(DialogAnimationType animType)
    {
        yield return AnimateDialoguePanel(animType, false);
        HideDialogue();
    }

    public void AdvanceDialogue()
    {
        if (_isTyping)
        {
            SkipTyping();
        }
        else if (_waitingForInput)
        {
            _waitingForInput = false;
        }
    }

    public void HideDialogue()
    {
        // Store current dialog before clearing
        DialogData dialogToComplete = _currentDialog;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        // If there was a current dialog and events haven't fired yet, fire them now
        if (dialogToComplete != null && _isTyping)
        {
            OnDialogueComplete?.Invoke(dialogToComplete);
        }

        _currentDialog = null;
        _isTyping = false;
        _waitingForInput = false;
    }

    public bool IsDialoguePlaying()
    {
        return _isTyping || _waitingForInput || _isPlayingQueue;
    }

    public DialogData GetCurrentDialogue()
    {
        return _currentDialog;
    }

    public List<DialogData> GetAllDialogues()
    {
        return new List<DialogData>(allDialogues);
    }

    private IEnumerator AnimateDialoguePanel(DialogAnimationType animType, bool show)
    {
        if (dialoguePanelCanvasGroup == null) yield break;

        RectTransform panelRect = dialoguePanel.GetComponent<RectTransform>();

        switch (animType)
        {
            case DialogAnimationType.FadeIn:
                if (show)
                {
                    dialoguePanelCanvasGroup.alpha = 0f;
                    dialoguePanelCanvasGroup.DOFade(1f, animationDuration);
                }
                else
                {
                    dialoguePanelCanvasGroup.DOFade(0f, animationDuration);
                }

                break;

            case DialogAnimationType.SlideFromBottom:
                if (show)
                {
                    Vector2 originalPos = panelRect.anchoredPosition;
                    panelRect.anchoredPosition = new Vector2(originalPos.x, originalPos.y - 300f);
                    panelRect.DOAnchorPos(originalPos, animationDuration).SetEase(Ease.OutBack);
                }
                else
                {
                    panelRect.DOAnchorPosY(panelRect.anchoredPosition.y - 300f, animationDuration).SetEase(Ease.InBack);
                }

                break;

            case DialogAnimationType.Pop:
                if (show)
                {
                    dialoguePanel.transform.localScale = Vector3.zero;
                    dialoguePanel.transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);
                }
                else
                {
                    dialoguePanel.transform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InBack);
                }

                break;
        }

        yield return new WaitForSeconds(animationDuration);
    }

    #endregion


    public void SkipTyping()
    {
        if (_isTyping && canSkipTyping)
        {
            _skipRequested = true;
        }
    }


    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            AdvanceDialogue();
        }
    }


    public void PlayDialogueGroup(string groupName)
    {
        DialogueGroup group = Resources.Load<DialogueGroup>($"Dialogues/{groupName}");

        if (group == null)
        {
            Debug.LogWarning($"Dialogue group '{groupName}' not found!");
            return;
        }

        EnqueueDialogues(group.dialogues);
    }


    public void PlayDialogueGroup(DialogueGroup group)
    {
        if (group == null)
        {
            Debug.LogWarning("Dialogue group is null!");
            return;
        }

        group.PlayGroup();
    }


    public void EnqueueDialoguesByGroup(string groupName)
    {
        List<DialogData> groupDialogues = allDialogues
            .Where(d => d.groupName == groupName)
            .OrderBy(d => d.sortingNumber)
            .ToList();

        if (groupDialogues.Count == 0)
        {
            Debug.LogWarning($"No dialogues found in group '{groupName}'!");
            return;
        }

        EnqueueDialogues(groupDialogues);
    }
}