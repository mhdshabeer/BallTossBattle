using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [Header("Target Settings")]
    public int health = 1;
    public int pointValue = 10;
    public float wobbleAmount = 0.5f;
    public float wobbleSpeed = 1f;
    
    [Header("Visual Feedback")]
    public Color hitColor = Color.red;
    public float hitColorDuration = 0.1f;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 startPosition;
    private GameManager gameManager;
    
    private void Start()
    {
        // Get references
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameManager.Instance;
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Save starting position for wobble effect
        startPosition = transform.position;
        
        // Start wobble animation
        StartCoroutine(WobbleAnimation());
    }
    
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        
        // Visual feedback
        StartCoroutine(HitFeedback());
        
        // Check if target is destroyed
        if (health <= 0)
        {
            // Notify game manager
            if (gameManager != null)
            {
                gameManager.TargetHit(this);
            }
        }
    }
    
    private IEnumerator HitFeedback()
    {
        // Change color to hit color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hitColor;
        }
        
        // Wait for duration
        yield return new WaitForSeconds(hitColorDuration);
        
        // Change back to original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
    
    private IEnumerator WobbleAnimation()
    {
        float timeOffset = Random.Range(0f, 2f * Mathf.PI);  // Random starting point
        
        while (true)
        {
            // Create a wobble effect
            float xOffset = Mathf.Sin(Time.time * wobbleSpeed + timeOffset) * wobbleAmount;
            
            // Apply to position
            transform.position = new Vector3(
                startPosition.x + xOffset,
                startPosition.y,
                startPosition.z
            );
            
            yield return null;
        }
    }
}
