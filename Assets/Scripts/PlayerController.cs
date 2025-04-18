using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Control Settings")]
    public float throwStrengthMin = 5f;
    public float throwStrengthMax = 15f;
    public float throwChargeSpeed = 5f;
    public float aimSensitivity = 2f;
    
    [Header("References")]
    public Transform aimIndicator;
    public Transform cameraTransform;
    public LineRenderer trajectoryLine;
    
    // Internal state
    private float currentThrowStrength;
    private bool isCharging = false;
    private Vector3 aimDirection;
    private GameManager gameManager;
    private ARManager arManager;
    private bool isARMode = false;
    
    // Touch/drag handling
    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    
    private void Start()
    {
        // Get references
        gameManager = GameManager.Instance;
        arManager = FindObjectOfType<ARManager>();
        
        // Initialize
        aimDirection = transform.forward;
        currentThrowStrength = throwStrengthMin;
        
        // Set starting aim indicator
        UpdateAimIndicator();
    }
    
    private void Update()
    {
        // Check if game is active
        if (gameManager == null || gameManager.currentGameState != GameManager.GameState.Playing)
        {
            return;
        }
        
        // Update AR mode status
        isARMode = gameManager.isARMode;
        
        // Handle different control schemes based on mode
        if (isARMode)
        {
            HandleARControls();
        }
        else
        {
            Handle2DControls();
        }
    }
    
    // 2D Mode controls
    private void Handle2DControls()
    {
        // Mouse input for aiming
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float mouseX = Input.GetAxis("Mouse X") * aimSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * aimSensitivity;
            
            // Adjust aim direction
            aimDirection = Quaternion.Euler(-mouseY, mouseX, 0) * aimDirection;
            
            // Update aim indicator
            UpdateAimIndicator();
        }
        
        // Throw charging with left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // Start charging
            isCharging = true;
            currentThrowStrength = throwStrengthMin;
        }
        
        // Charge while holding
        if (isCharging)
        {
            // Increase throw strength
            currentThrowStrength += throwChargeSpeed * Time.deltaTime;
            currentThrowStrength = Mathf.Clamp(currentThrowStrength, throwStrengthMin, throwStrengthMax);
            
            // Update trajectory
            UpdateTrajectory();
        }
        
        // Release throw
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            // Throw the ball
            ThrowBall();
            
            // Reset charging
            isCharging = false;
            
            // Hide trajectory
            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = false;
            }
        }
    }
    
    // AR Mode controls
    private void HandleARControls()
    {
        // Touch input for AR
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Start touch
                    lastTouchPosition = touch.position;
                    isDragging = true;
                    
                    // Start charging
                    isCharging = true;
                    currentThrowStrength = throwStrengthMin;
                    break;
                    
                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        // Calculate delta
                        Vector2 touchDelta = touch.position - lastTouchPosition;
                        lastTouchPosition = touch.position;
                        
                        // Use delta for aiming
                        // In AR we use the AR camera forward direction as base, then offset based on touch
                        if (cameraTransform != null)
                        {
                            aimDirection = cameraTransform.forward;
                            
                            // Apply touch delta to adjust aim (horizontal only for simplicity in AR)
                            float sensitivity = aimSensitivity * 0.02f; // Lower sensitivity for touch
                            aimDirection = Quaternion.Euler(0, touchDelta.x * sensitivity, 0) * aimDirection;
                        }
                        
                        // Update aim indicator
                        UpdateAimIndicator();
                    }
                    
                    // Increase throw strength while dragging
                    if (isCharging)
                    {
                        currentThrowStrength += throwChargeSpeed * Time.deltaTime;
                        currentThrowStrength = Mathf.Clamp(currentThrowStrength, throwStrengthMin, throwStrengthMax);
                        
                        // Update trajectory
                        UpdateTrajectory();
                    }
                    break;
                    
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isDragging && isCharging)
                    {
                        // Throw the ball
                        ThrowBall();
                        
                        // Reset
                        isDragging = false;
                        isCharging = false;
                        
                        // Hide trajectory
                        if (trajectoryLine != null)
                        {
                            trajectoryLine.enabled = false;
                        }
                    }
                    break;
            }
        }
        
        // In AR mode, we also allow to place targets on surfaces
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // A simple tap (not drag) can place targets in AR mode
            if (!isDragging && !isCharging)
            {
                // Request a hit test for target placement
                if (arManager != null)
                {
                    arManager.RequestARHitTest();
                }
            }
        }
    }
    
    // Throw the ball with current strength and direction
    private void ThrowBall()
    {
        if (gameManager != null)
        {
            gameManager.ThrowBall(aimDirection, currentThrowStrength);
        }
    }
    
    // Update aim indicator position/rotation
    private void UpdateAimIndicator()
    {
        if (aimIndicator != null)
        {
            aimIndicator.forward = aimDirection;
        }
    }
    
    // Update trajectory preview
    private void UpdateTrajectory()
    {
        if (trajectoryLine == null)
        {
            return;
        }
        
        // Enable trajectory line
        trajectoryLine.enabled = true;
        
        // Calculate trajectory points
        const int numPoints = 20;
        Vector3[] points = new Vector3[numPoints];
        
        // Starting position
        Vector3 startPos = transform.position;
        Vector3 startVelocity = aimDirection * currentThrowStrength;
        
        // Gravity
        float gravity = Physics.gravity.y;
        
        // Time step
        float timeStep = 0.1f;
        
        // Calculate trajectory points
        for (int i = 0; i < numPoints; i++)
        {
            float time = i * timeStep;
            
            // Position formula with gravity: p = p0 + v0*t + 0.5*g*t^2
            points[i] = startPos + 
                startVelocity * time + 
                0.5f * Physics.gravity * time * time;
        }
        
        // Set line renderer points
        trajectoryLine.positionCount = numPoints;
        trajectoryLine.SetPositions(points);
    }
}