using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARManager : MonoBehaviour
{
    [Header("AR Components")]
    public Camera arCamera;
    public Transform arSessionOrigin;
    
    [Header("UI Elements")]
    public GameObject arPromptPanel;
    public GameObject arNotSupportedPanel;
    
    // AR state
    private bool isARSupported = false;
    private bool isAREnabled = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // Check if AR is supported
        CheckARSupport();
        
        // Initially disable AR components
        if (arCamera != null)
        {
            arCamera.gameObject.SetActive(false);
        }
        
        if (arSessionOrigin != null)
        {
            arSessionOrigin.gameObject.SetActive(false);
        }
    }
    
    // Check if AR is supported on this device
    private void CheckARSupport()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        // In WebGL builds, check WebXR support
        isARSupported = CheckWebXRSupport();
        #else
        // For testing in editor, assume AR is supported
        isARSupported = true;
        #endif
    }
    
    private bool CheckWebXRSupport()
    {
        // Use WebGLARBridge to check support
        WebGLARBridge arBridge = WebGLARBridge.Instance;
        if (arBridge != null)
        {
            return arBridge.IsWebXRSupported();
        }
        
        // Fallback for testing
        return true;
    }
    
    public void StartAR()
    {
        if (!isARSupported)
        {
            ShowARNotSupportedMessage();
            return;
        }
        
        // Start AR session
        isAREnabled = true;
        
        // Initialize WebXR through bridge
        WebGLARBridge arBridge = WebGLARBridge.Instance;
        if (arBridge != null)
        {
            arBridge.StartAR();
        }
        
        // Setup AR camera and components
        SetupARComponents();
    }
    
    private void StartWebXRSession()
    {
        // This is handled by WebGLARBridge
        WebGLARBridge arBridge = WebGLARBridge.Instance;
        if (arBridge != null)
        {
            arBridge.StartAR();
        }
    }
    
    private void SetupARComponents()
    {
        // Configure AR camera and components
        if (arCamera != null)
        {
            // Enable AR camera
            arCamera.gameObject.SetActive(true);
            
            // Disable main camera
            Camera mainCamera = Camera.main;
            if (mainCamera != null && mainCamera != arCamera)
            {
                mainCamera.gameObject.SetActive(false);
            }
        }
        
        // Enable AR session origin
        if (arSessionOrigin != null)
        {
            arSessionOrigin.gameObject.SetActive(true);
        }
        
        // Notify game manager that AR is ready
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            // We could have a callback here to setup the game in AR mode
        }
    }
    
    public void StopAR()
    {
        isAREnabled = false;
        
        // End WebXR session through bridge
        WebGLARBridge arBridge = WebGLARBridge.Instance;
        if (arBridge != null)
        {
            arBridge.StopAR();
        }
        
        // Reset to non-AR camera and components
        ResetToNonARComponents();
    }
    
    private void StopWebXRSession()
    {
        // This is handled by WebGLARBridge
        WebGLARBridge arBridge = WebGLARBridge.Instance;
        if (arBridge != null)
        {
            arBridge.StopAR();
        }
    }
    
    private void ResetToNonARComponents()
    {
        // Disable AR camera
        if (arCamera != null)
        {
            arCamera.gameObject.SetActive(false);
        }
        
        // Enable main camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera != arCamera)
        {
            mainCamera.gameObject.SetActive(true);
        }
        
        // Disable AR session origin
        if (arSessionOrigin != null)
        {
            arSessionOrigin.gameObject.SetActive(false);
        }
    }
    
    public void ShowARPrompt()
    {
        if (arPromptPanel != null)
        {
            arPromptPanel.SetActive(true);
        }
    }
    
    public void HideARPrompt()
    {
        if (arPromptPanel != null)
        {
            arPromptPanel.SetActive(false);
        }
    }
    
    public void ShowARNotSupportedMessage()
    {
        if (arNotSupportedPanel != null)
        {
            arNotSupportedPanel.SetActive(true);
        }
    }
    
    // Method to handle AR hit test results (would be called from JavaScript)
    public void HandleARHitResult(Vector3 hitPosition)
    {
        // Log hit position for debugging
        Debug.Log("AR Hit detected at: " + hitPosition);
        
        // Place target at hit position if in AR mode
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && gameManager.isARMode && isAREnabled)
        {
            // Place a target at hit position
            gameManager.SpawnTargetAtPosition(hitPosition);
        }
    }
    
    // Method to request an AR hit test
    public void RequestARHitTest()
    {
        WebGLARBridge arBridge = WebGLARBridge.Instance;
        if (arBridge != null && isAREnabled)
        {
            arBridge.RequestHitTest();
        }
    }
    
    // Public method to check if AR is supported
    public bool IsARSupported()
    {
        return isARSupported;
    }
}