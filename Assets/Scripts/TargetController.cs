using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [Header("Target Properties")]
    public int pointValue = 10;
    public float spinSpeed = 90f;
    public float moveDistance = 0f;
    public float moveSpeed = 1f;
    public bool randomizeRotation = true;
    public bool randomizeSize = false;
    
    [Header("Effects")]
    public GameObject explosionPrefab;
    public ParticleSystem hitParticles;
    public AudioSource hitAudio;
    
    // Visual properties
    [Header("Visual Settings")]
    public bool changeColorOnHit = true;
    public Color hitColor = Color.red;
    public float flashDuration = 0.2f;
    
    // Private variables
    private Vector3 startPosition;
    private Vector3 moveDirection;
    private Renderer targetRenderer;
    private Material targetMaterial;
    private Color originalColor;
    private AudioManager audioManager;
    
    private void Awake()
    {
        // Get references
        targetRenderer = GetComponent<Renderer>();
        
        if (targetRenderer != null)
        {
            targetMaterial = targetRenderer.material;
            originalColor = targetMaterial.color;
        }
    }
    
    private void Start()
    {
        // Get audio manager
        audioManager = FindObjectOfType<AudioManager>();
        
        // Save starting position
        startPosition = transform.position;
        
        // Randomize initial rotation if enabled
        if (randomizeRotation)
        {
            transform.rotation = Quaternion.Euler(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );
        }
        
        // Randomize size if enabled
        if (randomizeSize)
        {
            float randomScale = Random.Range(0.8f, 1.5f);
            transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }
        
        // Set random movement direction
        moveDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0f
        ).normalized;
    }
    
    private void Update()
    {
        // Rotate the target
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        
        // Move the target if movement is enabled
        if (moveDistance > 0f)
        {
            // Calculate new position
            Vector3 newPosition = startPosition + moveDirection * Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            
            // Apply new position
            transform.position = newPosition;
        }
    }
    
    // Called when a ball hits the target
    public void OnHit()
    {
        // Show hit effects
        if (hitParticles != null)
        {
            hitParticles.Play();
        }
        
        // Play sound
        if (audioManager != null)
        {
            audioManager.PlayTargetHitSound();
        }
        else if (hitAudio != null)
        {
            hitAudio.Play();
        }
        
        // Visual feedback
        if (changeColorOnHit && targetMaterial != null)
        {
            StartCoroutine(FlashColor());
        }
    }
    
    // Get the point value for this target
    public int GetPointValue()
    {
        return pointValue;
    }
    
    // Destroy the target
    public void DestroyTarget()
    {
        // Call OnHit for effects
        OnHit();
        
        // Spawn explosion effect if available
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        
        // Hide the target immediately
        if (targetRenderer != null)
        {
            targetRenderer.enabled = false;
        }
        
        // Destroy the game object after a small delay
        Destroy(gameObject, 0.2f);
    }
    
    // Flash the target color when hit
    private IEnumerator FlashColor()
    {
        // Change to hit color
        targetMaterial.color = hitColor;
        
        // Wait for flash duration
        yield return new WaitForSeconds(flashDuration);
        
        // Restore original color
        targetMaterial.color = originalColor;
    }
}