using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    public UIManager uiManager;

    public float rotationSpeed = 100f;

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
            Destroy(gameObject);
        }

    }
}
