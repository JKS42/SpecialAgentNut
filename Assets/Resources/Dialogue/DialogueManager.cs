using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public string fileName;

    private DialogueQueue queue;

    void Start()
    {
        LoadDialogue();
        ShowNext();
    }

    void LoadDialogue()
    {
        TextAsset json = Resources.Load<TextAsset>("Dialogues/" + fileName);
        DialogueData data = JsonUtility.FromJson<DialogueData>(json.text);

        queue = new DialogueQueue(data.dialogues.Length);

        foreach (DialogueItem item in data.dialogues)
        {
            queue.Enqueue(item);
        }
    }

    public void ShowNext()
    {
        if (queue.IsEmpty())
        {
            return;
        }

        DialogueItem item = queue.Dequeue();
        UpdateUI(item);
    }

    void UpdateUI(DialogueItem item)
    {
        // Set text + icon here
    }
}