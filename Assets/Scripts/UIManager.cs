using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject mainMenuPanel;
    public GameObject gameplayPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject arModePromptPanel;
    
    [Header("UI Text Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI ballCountText;
    public TextMeshProUGUI finalScoreText;
    
    [Header("UI Button References")]
    public Button play2DButton;
    public Button playARButton;
    public Button pauseButton;
    public Button resumeButton;
    public Button quitToMenuButton;
    public Button arAcceptButton;
    public Button arDeclineButton;
    
    // Private variables
    private GameManager gameManager;
    private ARManager arManager;
    
    private void Start()
    {
        // Get references
        gameManager = GameManager.Instance;
        arManager = FindObjectOfType<ARManager>();
        
        // Setup button listeners
        SetupButtonListeners();
        
        // Initialize UI
        HideAllPanels();
        ShowMainMenu();
        
        // Check if AR is supported
        UpdateARButtonStatus();
    }
    
    private void SetupButtonListeners()
    {
        // Main menu buttons
        if (play2DButton != null)
        {
            play2DButton.onClick.AddListener(() => {
                if (gameManager != null)
                {
                    gameManager.StartGame(false);
                }
            });
        }
        
        if (playARButton != null)
        {
            playARButton.onClick.AddListener(() => {
                // First show AR prompt
                ShowARPrompt();
            });
        }
        
        // AR Prompt buttons
        if (arAcceptButton != null)
        {
            arAcceptButton.onClick.AddListener(() => {
                // Hide prompt and start AR mode
                HideARPrompt();
                
                if (gameManager != null)
                {
                    gameManager.StartGame(true);
                }
            });
        }
        
        if (arDeclineButton != null)
        {
            arDeclineButton.onClick.AddListener(() => {
                // Just hide prompt and go back to menu
                HideARPrompt();
            });
        }
        
        // Gameplay buttons
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(() => {
                if (gameManager != null)
                {
                    gameManager.PauseGame();
                }
            });
        }
        
        // Pause menu buttons
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(() => {
                if (gameManager != null)
                {
                    gameManager.ResumeGame();
                }
            });
        }
        
        // Shared buttons
        if (quitToMenuButton != null)
        {
            quitToMenuButton.onClick.AddListener(() => {
                if (gameManager != null)
                {
                    gameManager.SetGameState(GameManager.GameState.MainMenu);
                }
            });
        }
    }
    
    // Panel management
    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (arModePromptPanel != null) arModePromptPanel.SetActive(false);
    }
    
    public void ShowMainMenu()
    {
        HideAllPanels();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        
        // Update high score text
        UpdateHighScoreDisplay();
        
        // Check AR support and update button
        UpdateARButtonStatus();
    }
    
    public void ShowGameplayUI()
    {
        HideAllPanels();
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
    }
    
    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }
    
    public void ShowGameOverScreen(int finalScore)
    {
        HideAllPanels();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        
        // Set final score text
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore;
        }
    }
    
    public void ShowARPrompt()
    {
        if (arModePromptPanel != null) arModePromptPanel.SetActive(true);
    }
    
    public void HideARPrompt()
    {
        if (arModePromptPanel != null) arModePromptPanel.SetActive(false);
    }
    
    // UI updates
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
    
    public void UpdateHighScoreDisplay()
    {
        if (highScoreText != null && gameManager != null)
        {
            highScoreText.text = "High Score: " + gameManager.highScore;
        }
    }
    
    public void UpdateBallCount(int count)
    {
        if (ballCountText != null)
        {
            ballCountText.text = "Balls: " + count;
        }
    }
    
    public void UpdateTimer(float time)
    {
        if (timerText != null)
        {
            int seconds = Mathf.FloorToInt(time);
            timerText.text = "Time: " + seconds + "s";
        }
    }
    
    private void UpdateARButtonStatus()
    {
        // Check if AR is supported
        bool arSupported = true;
        
        if (arManager != null)
        {
            arSupported = arManager.IsARSupported();
        }
        
        // Enable/disable AR button based on support
        if (playARButton != null)
        {
            playARButton.interactable = arSupported;
            
            // Add visual indication that AR is not available
            ColorBlock colors = playARButton.colors;
            if (!arSupported)
            {
                colors.normalColor = new Color(0.5f, 0.5f, 0.5f);
                playARButton.GetComponentInChildren<TextMeshProUGUI>().text = "AR Mode (Not Supported)";
            }
            else
            {
                colors.normalColor = Color.white;
                playARButton.GetComponentInChildren<TextMeshProUGUI>().text = "AR Mode";
            }
            
            playARButton.colors = colors;
        }
    }
}