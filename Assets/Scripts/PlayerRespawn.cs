using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerRespawn : MonoBehaviour
{
    public GameObject respawnPosition;
    public float thresholdY;
    public float counter;
    public GameObject[] health;
    public GameObject GameOverScreen;
    public InputActionAsset inputActions;
    private InputAction pauseAction;
    private InputActionMap playerMap;
    public GameOverScript gameOverScript;
    bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        counter = 3f;
        playerMap = inputActions.FindActionMap("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (transform.position.y < thresholdY)
        {
            transform.position = respawnPosition.transform.position;
            counter -= 1f;

            if (counter <= 2f) health[2].SetActive(false);
            if (counter <= 1f) health[1].SetActive(false);

            if (counter <= 0f)
            {
                health[0].SetActive(false);
                isDead = true;

                Debug.Log("Game Over");

                gameOverScript.GameOver();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerMap.Disable();

            }
        }
    }

    public void GainLife()
    {
        // Only gain life if not at max
        if (counter < health.Length)
        {
            counter += 1f;

            // Re-enable hearts up to current counter
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

}
