using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string fileName;
    public DialogueManager dialogueManager; // assign in Inspector

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(fileName);
            }
            else
            {
                Debug.LogError("DialogueManager not assigned on trigger: " + gameObject.name);
            }
        }
    }
}
