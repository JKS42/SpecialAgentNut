using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class UIManager : MonoBehaviour
{
    [Header("Lives")]
    public Image[] hearts;      
    public int maxLives = 3;
    private int currentLives;
    private CoinPickup coinPickup; 

    [Header("Score")]
    public TMP_Text scoreText;  
    private int score = 0;

    void Start()
    {
        currentLives = maxLives;
        UpdateLivesUI();
        UpdateScoreUI();
    }

    // Call this to reduce a life
    public void LoseLife()
    {
        currentLives--;
        if (currentLives < 0) currentLives = 0;
        UpdateLivesUI();
    }

    // Call this to add a life
    public void GainLife()
    {
        currentLives++;
        if (currentLives > maxLives) currentLives = maxLives;
        UpdateLivesUI();
    }

    // Call this to add score
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateLivesUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // Enable hearts up to currentLives
            hearts[i].enabled = i < currentLives;
        }
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }
}
