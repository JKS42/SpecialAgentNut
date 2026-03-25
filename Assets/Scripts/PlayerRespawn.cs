using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public float thresholdY;
    public GameObject respawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position.y < thresholdY)
        {
            transform.position = respawnPosition.transform.position;
        }
    }
    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPosition.transform.position = newRespawnPoint;
    }
}
