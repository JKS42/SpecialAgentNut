using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameOverScript : MonoBehaviour
{
    public GameObject GameOverScreen;
    public string mainMenuSceneName = "MainMenu";
    public InputActionAsset inputActions;

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void quitGame()
    {
        Application.Quit();
    }
    public void GameOver()
    {
        GameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}
