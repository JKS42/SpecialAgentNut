using UnityEngine;

public class AutoDialogueStarter : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public string fileName;

    // displays dialogue automatically when the scene starts
    void Start()
    {

        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(fileName);
        }
    }
}
