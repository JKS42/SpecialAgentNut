using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // Add your coin collection logic here, such as incrementing the player's coin count.
            Debug.Log("Coin collected!");
            // Destroy the coin object after collection
            Destroy(gameObject);
            
        }
    }
}
