using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject howToPlayPanel;
    public GameObject levelSelectPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene("Beginner_Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowHowToPlay()
    {
        howToPlayPanel.SetActive(true);
    }

    public void HideHowToPlay()
    {
        howToPlayPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        levelSelectPanel.SetActive(true);
    }

    public void HideLevelSelect()
    {
        levelSelectPanel.SetActive(false);
    }

    public void AdvancedLevel()
    {
        SceneManager.LoadScene("Advanced_Level");
    }
    public void ExpertLevel()
    {
        SceneManager.LoadScene("Expert_Level");
    }
}
