using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public float thresholdY;
    public GameObject respawnPosition;

    private bool hasLostLifeThisFall = false;

    // Update is called once per frame
    [System.Obsolete]
    void FixedUpdate()
    {
        if (transform.position.y < thresholdY)
        {
            if (!hasLostLifeThisFall)
            {
                UIManager uiManager = FindObjectOfType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.LoseLife();
                }
                else
                {
                    Debug.LogWarning("UIManager not found in scene. Lives not updated.");
                }

                hasLostLifeThisFall = true;
            }

            transform.position = respawnPosition.transform.position;
        }
        else
        {
            // Reset fall lock once player is back above threshold
            hasLostLifeThisFall = false;
        }
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPosition.transform.position = newRespawnPoint;
    }
}
