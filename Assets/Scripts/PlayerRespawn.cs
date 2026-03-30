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
            if(counter == 2f)
            {
                health[2].SetActive(false);
            }
            else if(counter == 1f)
            {
                health[1].SetActive(false);
            }
            else if(counter == 0f)
            {
                health[0].SetActive(false);
                GameOverScreen.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                playerMap.Disable();
            }
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
