using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Beginner_Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenHowToPlay()
    {
        
    }
}
