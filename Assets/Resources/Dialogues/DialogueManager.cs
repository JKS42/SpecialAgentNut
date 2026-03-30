using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Canvas dialogueCanvas;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public Image iconImage;
    public Button nextButton;
    public MonoBehaviour playerMovement;

    private Queue<DialogueItem> dialogueQueue;
    private bool isActive = false;

    void Start()
    {

        if (dialogueCanvas != null)
            dialogueCanvas.enabled = false;

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ShowNext);
        }
    }

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Starts a dialogue sequence from a JSON file
    /// Optionally filters dialogue by a specific title
    /// </summary>
    public void StartDialogue(string fileName, string onlyTitle = null)
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


        if (playerMovement != null)
            playerMovement.enabled = false;

        LoadDialogue(fileName);

        if (dialogueQueue == null)
            return;

        // Optional: filter dialogue to only include entries with a specific title
        if (!string.IsNullOrEmpty(onlyTitle))
        {
            Queue<DialogueItem> filtered = new Queue<DialogueItem>();

            foreach (DialogueItem item in dialogueQueue)
            {
                if (item.title == onlyTitle)
                    filtered.Enqueue(item);
            }

            dialogueQueue = filtered;
        }


        if (dialogueCanvas != null)
            dialogueCanvas.enabled = true;

        isActive = true;

        ShowNext();
    }

    /// <summary>
    /// Loads dialogue JSON from Resources/Dialogues folder
    /// </summary>
    private void LoadDialogue(string fileName)
    {
        TextAsset json = Resources.Load<TextAsset>("Dialogues/" + fileName);

        if (json == null)
        {
            Debug.LogError("Dialogue JSON not found: " + fileName);
            dialogueQueue = new Queue<DialogueItem>();
            return;
        }

        // Deserialize JSON into DialogueData object
        DialogueData data = JsonUtility.FromJson<DialogueData>(json.text);

        // Convert list into a queue for sequential access
        dialogueQueue = new Queue<DialogueItem>(data.dialogues);
    }

    /// <summary>
    /// Displays the next dialogue item in the queue
    /// </summary>
    public void ShowNext()
    {
        if (dialogueQueue == null || dialogueQueue.Count == 0)
        {

            // If no more dialogue, end the conversation
            if (dialogueCanvas != null)
                dialogueCanvas.enabled = false;

            isActive = false;

            if (playerMovement != null)
                playerMovement.enabled = true;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            return;
        }

        // Get next dialogue item
        DialogueItem current = dialogueQueue.Dequeue();

        titleText.text = current.title;
        messageText.text = current.message;

        if (!string.IsNullOrEmpty(current.icon))
        {
            Sprite iconSprite = Resources.Load<Sprite>("Icons/" + current.icon);

            if (iconSprite != null)
                iconImage.sprite = iconSprite;
            else
                iconImage.sprite = null;
        }
        else
        {
            iconImage.sprite = null;
        }
    }

    // Returns whether a dialogue is currently active
    public bool IsDialogueActive()
    {
        return isActive;
    }
}