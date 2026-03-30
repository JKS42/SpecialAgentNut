using UnityEngine;

public class AutoDialogueStarter : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public string fileName;

    void Start()
    {

        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(fileName);
        }
    }
}
