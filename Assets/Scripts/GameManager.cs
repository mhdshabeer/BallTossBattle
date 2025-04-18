using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public bool isARMode = false;
    public int currentScore = 0;
    public int highScore = 0;
    public int remainingBalls = 5;
    public float gameTimer = 60f;
    public GameState currentGameState = GameState.MainMenu;
    
    [Header("Prefabs")]
    public GameObject ballPrefab;
    public GameObject targetPrefab;
    
    [Header("References")]
    public Transform ballSpawnPoint;
    public Transform targetParent;
    
    [Header("Game Parameters")]
    public float throwForce = 10f;
    public int pointsPerTarget = 10;
    public float targetSpawnInterval = 2f;
    
    // Private variables
    private List<GameObject> activeBalls = new List<GameObject>();
    private List<GameObject> activeTargets = new List<GameObject>();
    private ARManager arManager;
    private UIManager uiManager;
    private AudioManager audioManager;
    
    // Enumerations
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Get references
        arManager = FindObjectOfType<ARManager>();
        uiManager = FindObjectOfType<UIManager>();
        audioManager = FindObjectOfType<AudioManager>();
        
        // Initialize game
        ResetGame();
        SetGameState(GameState.MainMenu);
    }
    
    private void Update()
    {
        // Handle game timer if playing
        if (currentGameState == GameState.Playing)
        {
            if (gameTimer > 0)
            {
                gameTimer -= Time.deltaTime;
                
                // Update UI
                if (uiManager != null)
                {
                    uiManager.UpdateTimer(gameTimer);
                }
                
                // Check if time ran out
                if (gameTimer <= 0)
                {
                    EndGame();
                }
            }
        }
    }
    
    // Game state management
    public void SetGameState(GameState newState)
    {
        currentGameState = newState;
        
        switch (newState)
        {
            case GameState.MainMenu:
                // Show main menu UI
                if (uiManager != null)
                {
                    uiManager.ShowMainMenu();
                }
                
                // Clean up game objects
                ClearAllGameObjects();
                break;
                
            case GameState.Playing:
                // Show gameplay UI
                if (uiManager != null)
                {
                    uiManager.ShowGameplayUI();
                }
                
                // Start spawning targets in 2D mode
                if (!isARMode)
                {
                    StartCoroutine(SpawnTargetsRoutine());
                }
                break;
                
            case GameState.Paused:
                // Show pause menu
                if (uiManager != null)
                {
                    uiManager.ShowPauseMenu();
                }
                
                // Pause physics/gameplay
                Time.timeScale = 0f;
                break;
                
            case GameState.GameOver:
                // Show game over UI
                if (uiManager != null)
                {
                    uiManager.ShowGameOverScreen(currentScore);
                }
                
                // Clean up game objects
                ClearAllGameObjects();
                break;
        }
    }
    
    // Game control methods
    public void StartGame(bool arMode = false)
    {
        isARMode = arMode;
        
        // Reset game parameters
        ResetGame();
        
        // Setup game mode
        if (arMode)
        {
            StartARMode();
        }
        else
        {
            Start2DMode();
        }
        
        // Start the game
        SetGameState(GameState.Playing);
    }
    
    public void PauseGame()
    {
        if (currentGameState == GameState.Playing)
        {
            SetGameState(GameState.Paused);
        }
    }
    
    public void ResumeGame()
    {
        if (currentGameState == GameState.Paused)
        {
            Time.timeScale = 1f;
            SetGameState(GameState.Playing);
        }
    }
    
    public void EndGame()
    {
        // Check for high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            
            // Save high score
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        
        // Reset time scale if paused
        Time.timeScale = 1f;
        
        // Show game over screen
        SetGameState(GameState.GameOver);
    }
    
    public void ResetGame()
    {
        // Reset score and timer
        currentScore = 0;
        remainingBalls = 5;
        gameTimer = 60f;
        
        // Load saved high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
            uiManager.UpdateBallCount(remainingBalls);
            uiManager.UpdateTimer(gameTimer);
        }
        
        // Clear all game objects
        ClearAllGameObjects();
    }
    
    // Mode-specific setup
    private void Start2DMode()
    {
        // Ensure proper camera setup (use main camera, not AR camera)
        if (arManager != null)
        {
            arManager.StopAR();
        }
    }
    
    private void StartARMode()
    {
        // Initialize AR
        if (arManager != null)
        {
            arManager.StartAR();
        }
    }
    
    public void StopARMode()
    {
        isARMode = false;
        
        // Switch back to 2D mode if needed
        if (currentGameState == GameState.Playing)
        {
            Start2DMode();
        }
    }
    
    // Ball throwing
    public void ThrowBall(Vector3 direction, float force = -1)
    {
        // Check if we have balls remaining
        if (remainingBalls <= 0 || currentGameState != GameState.Playing)
        {
            return;
        }
        
        // Use default force if none specified
        if (force < 0)
        {
            force = throwForce;
        }
        
        // Create a new ball
        GameObject ball = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        
        // Apply force in the specified direction
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction.normalized * force, ForceMode.Impulse);
        }
        
        // Add to active balls list
        activeBalls.Add(ball);
        
        // Decrement ball count
        remainingBalls--;
        
        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateBallCount(remainingBalls);
        }
        
        // Play sound effect
        if (audioManager != null)
        {
            audioManager.PlayThrowSound();
        }
        
        // Check if we've run out of balls
        if (remainingBalls <= 0)
        {
            // End game after a short delay to allow the last ball to be thrown
            Invoke("EndGame", 3f);
        }
    }
    
    // Target spawning
    private IEnumerator SpawnTargetsRoutine()
    {
        while (currentGameState == GameState.Playing && !isARMode)
        {
            SpawnRandomTarget();
            yield return new WaitForSeconds(targetSpawnInterval);
        }
    }
    
    private void SpawnRandomTarget()
    {
        if (targetPrefab == null)
        {
            Debug.LogError("Target prefab is not assigned!");
            return;
        }
        
        // Generate random position for 2D mode
        float x = Random.Range(-5f, 5f);
        float y = Random.Range(1f, 4f);
        float z = Random.Range(8f, 12f);
        
        Vector3 position = new Vector3(x, y, z);
        
        // Create the target
        GameObject target = Instantiate(targetPrefab, position, Quaternion.identity, targetParent);
        
        // Add to active targets list
        activeTargets.Add(target);
        
        // Auto-remove after a delay
        StartCoroutine(DestroyTargetAfterDelay(target, 10f));
    }
    
    // For AR mode - spawn a target at a specific position
    public void SpawnTargetAtPosition(Vector3 position)
    {
        if (targetPrefab == null || !isARMode)
        {
            return;
        }
        
        // Create the target at the hit position
        GameObject target = Instantiate(targetPrefab, position, Quaternion.identity, targetParent);
        
        // Add to active targets list
        activeTargets.Add(target);
        
        // Auto-remove after a longer delay in AR mode
        StartCoroutine(DestroyTargetAfterDelay(target, 30f));
        
        // Play sound effect
        if (audioManager != null)
        {
            audioManager.PlayTargetPlacedSound();
        }
    }
    
    private IEnumerator DestroyTargetAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (target != null)
        {
            // Remove from active targets list
            activeTargets.Remove(target);
            
            // Destroy the target
            Destroy(target);
        }
    }
    
    // Score handling
    public void AddPoints(int points)
    {
        currentScore += points;
        
        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
        }
        
        // Play sound effect
        if (audioManager != null)
        {
            audioManager.PlayScoreSound();
        }
    }
    
    // Helper methods
    private void ClearAllGameObjects()
    {
        // Clear balls
        foreach (GameObject ball in activeBalls)
        {
            Destroy(ball);
        }
        activeBalls.Clear();
        
        // Clear targets
        foreach (GameObject target in activeTargets)
        {
            Destroy(target);
        }
        activeTargets.Clear();
    }
}