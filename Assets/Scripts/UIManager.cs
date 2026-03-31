using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Lives")]
    public Image[] hearts;   
    public int maxLives = 3;
    private int currentLives;

    [Header("Score")]
    public TMP_Text scoreText;
    private int score = 0;

    void Start()
    {
        currentLives = maxLives;
        UpdateLivesUI();
        UpdateScoreUI();
    }

    // Call to remove a life
    public void LoseLife()
    {
        currentLives--;
        if (currentLives < 0) currentLives = 0;
        UpdateLivesUI();
    }

    // Call to add a life
    public void GainLife()
    {
        currentLives++;
        if (currentLives > maxLives) currentLives = maxLives;
        UpdateLivesUI();
    }

    // Check if player is full health
    public bool IsFullHealth()
    {
        return currentLives >= maxLives;
    }

    // Call to add score
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    // Updates heart images
    void UpdateLivesUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentLives)
            {
                // Make heart visible
                hearts[i].enabled = true;
            }
            else
            {
                // Hide heart
                hearts[i].enabled = false;
            }
        }
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }
}
