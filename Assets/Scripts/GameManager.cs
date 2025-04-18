using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int playerScore = 0;
    public int targetScore = 100;
    public int ballsRemaining = 10;
    
    [Header("Game States")]
    public bool isGameStarted = false;
    public bool isGameOver = false;

    [Header("References")]
    public UIManager uiManager;
    public PlayerController playerController;
    public Transform targetSpawnArea;
    public GameObject targetPrefab;
    public List<GameObject> activeTargets = new List<GameObject>();

    [Header("Target Settings")]
    public int targetCount = 5;
    public float minTargetY = 1f;
    public float maxTargetY = 4f;
    public float minTargetX = -7f;
    public float maxTargetX = 7f;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Find references if not set in inspector
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();
        
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
            
        // Set initial game state
        isGameStarted = false;
        isGameOver = false;
        
        // Show start screen
        uiManager.ShowStartScreen();
    }

    public void StartGame()
    {
        // Reset game state
        playerScore = 0;
        ballsRemaining = 10;
        isGameStarted = true;
        isGameOver = false;
        
        // Update UI
        uiManager.UpdateScoreText(playerScore);
        uiManager.UpdateBallsRemainingText(ballsRemaining);
        uiManager.HideStartScreen();
        uiManager.ShowGameUI();
        
        // Spawn targets
        SpawnTargets();
        
        // Enable player controller
        playerController.enabled = true;
    }

    public void EndGame()
    {
        isGameStarted = false;
        isGameOver = true;
        
        // Update UI
        uiManager.ShowGameOverScreen(playerScore);
        
        // Disable player controller
        playerController.enabled = false;
        
        // Clear targets
        ClearTargets();
    }

    public void RestartGame()
    {
        // Start a new game
        StartGame();
    }

    public void AddScore(int points)
    {
        playerScore += points;
        uiManager.UpdateScoreText(playerScore);
        
        // Check if player reached target score
        if (playerScore >= targetScore)
        {
            EndGame();
        }
    }

    public void UseBall()
    {
        ballsRemaining--;
        uiManager.UpdateBallsRemainingText(ballsRemaining);
        
        // Check if player is out of balls
        if (ballsRemaining <= 0)
        {
            EndGame();
        }
    }

    private void SpawnTargets()
    {
        // Clear any existing targets
        ClearTargets();
        
        // Spawn new targets
        for (int i = 0; i < targetCount; i++)
        {
            // Random position within spawn area
            float randomX = Random.Range(minTargetX, maxTargetX);
            float randomY = Random.Range(minTargetY, maxTargetY);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0);
            
            // Instantiate target
            GameObject target = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
            activeTargets.Add(target);
        }
    }

    private void ClearTargets()
    {
        // Destroy all active targets
        foreach (GameObject target in activeTargets)
        {
            if (target != null)
            {
                Destroy(target);
            }
        }
        
        // Clear the list
        activeTargets.Clear();
    }

    public void TargetHit(TargetController target)
    {
        // Add score based on target value
        AddScore(target.pointValue);
        
        // Remove from active targets list
        activeTargets.Remove(target.gameObject);
        
        // Destroy the target
        Destroy(target.gameObject);
        
        // Check if all targets are cleared
        if (activeTargets.Count == 0)
        {
            // Spawn new targets
            SpawnTargets();
        }
    }
}
