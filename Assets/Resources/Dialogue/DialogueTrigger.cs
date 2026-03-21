using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string fileName; 

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            DialogueManager manager = Object.FindFirstObjectByType<DialogueManager>();
            manager.StartDialogue(fileName);
        }
    }
}
