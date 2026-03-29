using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public float thresholdY;
    public GameObject respawnPosition;
    public float counter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        counter = 3f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(transform.position.y < thresholdY)
        {
            transform.position = respawnPosition.transform.position;
            counter -= 1f;
            if(counter <= 0f)
            {
                // Game Over
                Debug.Log("Game Over");
                // You can add your game over logic here, such as reloading the scene or showing a game over screen.
            }
        }
    }
    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPosition.transform.position = newRespawnPoint;
    }
}
