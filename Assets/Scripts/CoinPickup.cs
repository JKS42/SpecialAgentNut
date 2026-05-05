using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public float rotationSpeed = 100f;

    public AudioClip pickupSound;
    public float soundVolume = 1f;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Coin collected!");


            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
            }

            UIManager uiManager = Object.FindObjectsByType<UIManager>(FindObjectsSortMode.None)[0];

            if (uiManager != null)
            {
                uiManager.AddScore(10);
            }
            else
            {
                Debug.LogWarning("UIManager not found in scene. Score not updated.");
            }

            Destroy(gameObject);
        }
    }
}
