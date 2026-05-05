using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRespawn : MonoBehaviour
{
    public GameObject respawnPosition;
    public float thresholdY;
    public float counter;
    public GameObject[] health;
    public GameObject GameOverScreen;
    public InputActionAsset inputActions;
    private InputActionMap playerMap;
    public GameOverScript gameOverScript;

    [Header("Game Over Sound")]
    public AudioClip gameOverSound;
    public float soundVolume = 1f;

    bool isDead = false;
    private float lastDamageTime;
    public float damageCooldown = 1f;

    void Start()
    {
        counter = 3f;
        playerMap = inputActions.FindActionMap("Player");
    }

    void FixedUpdate()
    {
        if (transform.position.y < thresholdY)
        {
            // Respawn and lose one life
            transform.position = respawnPosition.transform.position;
            counter -= 1f;

            if (counter <= 2f) health[2].SetActive(false);
            if (counter <= 1f) health[1].SetActive(false);

            // Only trigger death when no lives remain
            if (counter <= 0f)
            {
                health[0].SetActive(false);
                isDead = true;

                Debug.Log("Game Over");

                PlayGameOverSound();

                gameOverScript.GameOver();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerMap.Disable();
            }
        }
    }

    public void GainLife()
    {
        if (counter < health.Length)
        {
            counter += 1f;

            for (int i = 0; i < health.Length; i++)
            {
                health[i].SetActive(i < counter);
            }

            Debug.Log("Gained a life! Current lives: " + counter);
        }
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPosition.transform.position = newRespawnPoint;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (Time.time - lastDamageTime < damageCooldown) return;

        lastDamageTime = Time.time;
        counter -= amount;

        for (int i = 0; i < health.Length; i++)
        {
            health[i].SetActive(i < counter);
        }

        if (counter <= 0f)
        {
            isDead = true;

            Debug.Log("Game Over");

            PlayGameOverSound();

            gameOverScript.GameOver();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerMap.Disable();
        }
    }

    void PlayGameOverSound()
    {
        if (gameOverSound != null)
        {
            AudioSource.PlayClipAtPoint(gameOverSound, transform.position, soundVolume);
        }
    }
}
