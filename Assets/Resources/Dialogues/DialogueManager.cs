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
        else
        {
            Debug.LogError("Next Button not assigned in DialogueManager!");
        }
    }


    public void StartDialogue(string fileName, string onlyTitle = null)
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        LoadDialogue(fileName);

        if (dialogueQueue == null)
            return;


        if (!string.IsNullOrEmpty(onlyTitle))
        {
            Queue<DialogueItem> filtered = new Queue<DialogueItem>();
            foreach (DialogueItem item in dialogueQueue)
                if (item.title == onlyTitle)
                    filtered.Enqueue(item);

            dialogueQueue = filtered;
        }

        if (dialogueCanvas != null)
            dialogueCanvas.enabled = true;

        isActive = true;
        ShowNext();
    }

    private void LoadDialogue(string fileName)
    {
        TextAsset json = Resources.Load<TextAsset>("Dialogues/" + fileName);

        if (json == null)
        {
            Debug.LogError("Dialogue JSON not found: " + fileName);
            dialogueQueue = new Queue<DialogueItem>();
            return;
        }

        DialogueData data = JsonUtility.FromJson<DialogueData>(json.text);
        dialogueQueue = new Queue<DialogueItem>(data.dialogues);
    }

    public void ShowNext()
    {
        if (dialogueQueue == null || dialogueQueue.Count == 0)
        {
            if (dialogueCanvas != null)
                dialogueCanvas.enabled = false;

            isActive = false;


            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            return;
        }

        DialogueItem current = dialogueQueue.Dequeue();
        titleText.text = current.title;
        messageText.text = current.message;

        if (!string.IsNullOrEmpty(current.icon))
        {
            Sprite iconSprite = Resources.Load<Sprite>("Icons/" + current.icon);
            iconImage.sprite = iconSprite;
        }
        else
        {
            iconImage.sprite = null;
        }
    }

    public bool IsDialogueActive()
    {
        return isActive;
    }
}