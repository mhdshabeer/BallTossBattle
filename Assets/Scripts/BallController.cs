using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    public float lifetime = 5f;  // How long the ball exists before auto-destruction
    public int damage = 1;       // How much damage this ball does to targets
    
    [Header("References")]
    private AudioManager audioManager;
    
    private void Start()
    {
        // Find audio manager
        audioManager = FindObjectOfType<AudioManager>();
        
        // Start self-destruct timer
        Destroy(gameObject, lifetime);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit a target
        TargetController target = collision.gameObject.GetComponent<TargetController>();
        
        if (target != null)
        {
            // Hit a target, tell it to take damage
            target.TakeDamage(damage);
            
            // Play hit sound
            if (audioManager != null)
            {
                audioManager.PlayTargetHitSound();
            }
        }
        else
        {
            // Hit something else, play bounce sound
            if (audioManager != null)
            {
                audioManager.PlayBounceSound();
            }
        }
    }
    
    private void OnBecameInvisible()
    {
        // Destroy the ball when it goes off-screen to save resources
        Destroy(gameObject);
    }
}
