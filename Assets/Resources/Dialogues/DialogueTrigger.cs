using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string fileName;               
    public string onlyTitle;              
    public DialogueManager dialogueManager; 

    private bool hasTriggered = false;


    // Called when tag "Player" enters this trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            if (dialogueManager != null)
                dialogueManager.StartDialogue(fileName, onlyTitle);
            else
                Debug.LogError("DialogueManager not assigned on trigger: " + gameObject.name);
        }
    }
}
