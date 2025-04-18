using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float throwForceMin = 5f;
    public float throwForceMax = 15f;
    public float throwAngle = 45f;
    
    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    
    [Header("Throw Controls")]
    public float chargeRate = 10f;
    public float currentThrowForce;
    public bool isCharging = false;
    
    [Header("References")]
    private GameManager gameManager;
    private AudioManager audioManager;
    
    private void Start()
    {
        // Find references if not assigned in inspector
        gameManager = GameManager.Instance;
        audioManager = FindObjectOfType<AudioManager>();
        
        // Initialize
        currentThrowForce = throwForceMin;
    }
    
    private void Update()
    {
        // Only accept input if game is started
        if (!gameManager.isGameStarted || gameManager.isGameOver)
            return;
            
        // Input handling for mouse/touch
        HandleInput();
    }
    
    private void HandleInput()
    {
        // Start charging on mouse down / touch begin
        if (Input.GetMouseButtonDown(0) && !isCharging)
        {
            StartCharging();
        }
        
        // Continue charging while holding
        if (Input.GetMouseButton(0) && isCharging)
        {
            ContinueCharging();
        }
        
        // Release to throw
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            ThrowBall();
        }
    }
    
    private void StartCharging()
    {
        isCharging = true;
        currentThrowForce = throwForceMin;
    }
    
    private void ContinueCharging()
    {
        // Increase throw force while charging
        currentThrowForce += chargeRate * Time.deltaTime;
        
        // Clamp to max force
        currentThrowForce = Mathf.Clamp(currentThrowForce, throwForceMin, throwForceMax);
    }
    
    private void ThrowBall()
    {
        if (gameManager.ballsRemaining <= 0)
            return;
            
        // Create a new ball
        GameObject ball = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        
        // Get rigidbody
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            // Calculate direction vector with angle
            float radianAngle = throwAngle * Mathf.Deg2Rad;
            Vector2 throwDirection = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
            
            // Apply force
            rb.AddForce(throwDirection * currentThrowForce, ForceMode2D.Impulse);
            
            // Play throw sound
            if (audioManager != null)
            {
                audioManager.PlayThrowSound();
            }
            
            // Update game manager
            gameManager.UseBall();
        }
        
        // Reset charging state
        isCharging = false;
        currentThrowForce = throwForceMin;
    }
}
