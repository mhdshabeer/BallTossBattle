using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    
    [Header("Game UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ballsRemainingText;
    public Slider throwPowerSlider;
    
    [Header("Start UI")]
    public Button startButton;
    public TextMeshProUGUI titleText;
    
    [Header("Game Over UI")]
    public TextMeshProUGUI finalScoreText;
    public Button restartButton;
    
    private GameManager gameManager;
    private PlayerController playerController;
    
    private void Start()
    {
        // Get references
        gameManager = GameManager.Instance;
        playerController = FindObjectOfType<PlayerController>();
        
        // Setup button listeners
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            
        // Initial UI state
        HideAllPanels();
        ShowStartScreen();
    }
    
    private void Update()
    {
        // Update throw power slider based on player charging
        if (throwPowerSlider != null && playerController != null)
        {
            if (playerController.isCharging)
            {
                float normalizedForce = (playerController.currentThrowForce - playerController.throwForceMin) / 
                                       (playerController.throwForceMax - playerController.throwForceMin);
                throwPowerSlider.value = normalizedForce;
                throwPowerSlider.gameObject.SetActive(true);
            }
            else
            {
                throwPowerSlider.gameObject.SetActive(false);
            }
        }
    }
    
    private void HideAllPanels()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }
    
    public void ShowStartScreen()
    {
        HideAllPanels();
        if (startPanel != null) startPanel.SetActive(true);
    }
    
    public void ShowGameUI()
    {
        HideAllPanels();
        if (gamePanel != null) gamePanel.SetActive(true);
    }
    
    public void ShowGameOverScreen(int finalScore)
    {
        HideAllPanels();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Update final score
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + finalScore;
            }
        }
    }
    
    public void HideStartScreen()
    {
        if (startPanel != null) startPanel.SetActive(false);
    }
    
    public void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
    
    public void UpdateBallsRemainingText(int ballsRemaining)
    {
        if (ballsRemainingText != null)
            ballsRemainingText.text = "Balls: " + ballsRemaining;
    }
    
    private void OnStartButtonClicked()
    {
        if (gameManager != null)
            gameManager.StartGame();
    }
    
    private void OnRestartButtonClicked()
    {
        if (gameManager != null)
            gameManager.RestartGame();
    }
}
