using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public float rotationSpeed = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // Add your coin collection logic here, such as incrementing the player's coin count.
            Debug.Log("Coin collected!");
            
            // Increase UI score via UIManager
            UIManager uiManager = Object.FindObjectsByType<UIManager>(FindObjectsSortMode.None)[0]; // Get the first instance of UIManager in the scene
            if (uiManager != null)
            {
                uiManager.AddScore(10);
            }
            else
            {
                Debug.LogWarning("UIManager not found in scene. Score not updated.");
            }

            // Destroy the coin object after collection
            Destroy(gameObject);
        }
    }
}
