using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Ball Properties")]
    public float maxLifeTime = 5f;
    public float bounceForceReduction = 0.8f;
    public int maxBounces = 3;
    
    [Header("References")]
    public TrailRenderer trail;
    public ParticleSystem impactParticles;
    public AudioSource bounceAudio;
    
    // Private variables
    private int bounceCount = 0;
    private float creationTime;
    private Rigidbody rb;
    private GameManager gameManager;
    private AudioManager audioManager;
    
    private void Awake()
    {
        // Get components
        rb = GetComponent<Rigidbody>();
        
        // Mark creation time
        creationTime = Time.time;
    }
    
    private void Start()
    {
        // Get manager references
        gameManager = GameManager.Instance;
        audioManager = FindObjectOfType<AudioManager>();
        
        // Destroy ball after maximum lifetime
        Destroy(gameObject, maxLifeTime);
    }
    
    private void Update()
    {
        // If the ball is moving very slowly, destroy it
        if (rb != null && rb.velocity.magnitude < 0.1f && Time.time > creationTime + 1f)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Play bounce sound
        if (audioManager != null)
        {
            audioManager.PlayBounceSound();
        }
        else if (bounceAudio != null)
        {
            bounceAudio.Play();
        }
        
        // Spawn impact particles
        if (impactParticles != null)
        {
            impactParticles.Play();
        }
        
        // Check if it hit a target
        if (collision.gameObject.CompareTag("Target"))
        {
            HandleTargetHit(collision.gameObject);
        }
        else
        {
            // Regular bounce physics
            if (rb != null)
            {
                // Reduce velocity for more realistic bounces
                rb.velocity *= bounceForceReduction;
                
                // Increment bounce count
                bounceCount++;
                
                // Destroy ball if it has bounced too many times
                if (bounceCount >= maxBounces)
                {
                    StartCoroutine(DelayedDestroy(0.2f));
                }
            }
        }
    }
    
    private void HandleTargetHit(GameObject target)
    {
        // Get the target component
        TargetController targetController = target.GetComponent<TargetController>();
        
        if (targetController != null)
        {
            // Award points based on target
            int pointsAwarded = targetController.GetPointValue();
            
            if (gameManager != null)
            {
                gameManager.AddPoints(pointsAwarded);
            }
            
            // Destroy target
            targetController.DestroyTarget();
        }
        else if (gameManager != null)
        {
            // Basic point award if target doesn't have controller
            gameManager.AddPoints(gameManager.pointsPerTarget);
        }
        
        // Destroy ball after hitting target
        StartCoroutine(DelayedDestroy(0.1f));
    }
    
    private IEnumerator DelayedDestroy(float delay)
    {
        // Wait for specified delay
        yield return new WaitForSeconds(delay);
        
        // Disable physics and visuals
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        
        // Disable renderer
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
        
        // Disable trail
        if (trail != null)
        {
            trail.enabled = false;
        }
        
        // Destroy after another small delay
        Destroy(gameObject, 0.2f);
    }
}