using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLARBridge : MonoBehaviour
{
    // Singleton pattern
    public static WebGLARBridge Instance { get; private set; }
    
    [Header("AR Settings")]
    public bool arSessionStarted = false;
    
    // JavaScript functions declarations (implemented in WebGLARBridge.jslib)
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern bool CheckWebXRSupport();
    
    [DllImport("__Internal")]
    private static extern void StartWebXRSession();
    
    [DllImport("__Internal")]
    private static extern void StopWebXRSession();
    
    [DllImport("__Internal")]
    private static extern void GetWebXRHitResults();
    #endif
    
    // References
    private ARManager arManager;
    private GameManager gameManager;
    
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
        gameManager = GameManager.Instance;
    }
    
    // Called from JavaScript when WebXR returns hit test results
    public void OnWebXRHitResult(string hitPositionJson)
    {
        // Parse hit position from JSON
        Vector3 hitPosition = JsonUtility.FromJson<Vector3>(hitPositionJson);
        
        // Forward to ARManager
        if (arManager != null)
        {
            arManager.HandleARHitResult(hitPosition);
        }
    }
    
    // Called from JavaScript when WebXR session starts
    public void OnWebXRSessionStarted()
    {
        arSessionStarted = true;
        
        // Notify ARManager
        if (arManager != null)
        {
            // We might need to adjust camera settings or other AR parameters here
        }
    }
    
    // Called from JavaScript when WebXR session ends
    public void OnWebXRSessionEnded()
    {
        arSessionStarted = false;
        
        // Notify ARManager
        if (arManager != null && gameManager != null)
        {
            // Exit AR mode
            gameManager.StopARMode();
        }
    }
    
    // Public methods to be called from C#
    
    public bool IsWebXRSupported()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        return CheckWebXRSupport();
        #else
        // For testing in editor
        return true;
        #endif
    }
    
    public void StartAR()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        StartWebXRSession();
        #else
        // For testing in editor
        OnWebXRSessionStarted();
        #endif
    }
    
    public void StopAR()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        StopWebXRSession();
        #else
        // For testing in editor
        OnWebXRSessionEnded();
        #endif
    }
    
    public void RequestHitTest()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        GetWebXRHitResults();
        #else
        // For testing in editor, simulate a hit test result
        Vector3 simulatedHitPosition = new Vector3(
            Random.Range(-3f, 3f),
            0,
            Random.Range(1f, 5f)
        );
        
        // Call the hit result handler
        OnWebXRHitResult(JsonUtility.ToJson(simulatedHitPosition));
        #endif
    }
}