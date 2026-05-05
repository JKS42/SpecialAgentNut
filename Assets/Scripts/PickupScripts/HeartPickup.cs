using UnityEngine;

public class HeartPickup : MonoBehaviour
{

    public float rotationSpeed = 100f;

    public AudioClip pickupSound;
    public float soundVolume = 1f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerRespawn playerLife = other.GetComponent<PlayerRespawn>();

        if (playerLife != null)
        {
            playerLife.GainLife();

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
            }

            Destroy(gameObject);
        }
    }
}