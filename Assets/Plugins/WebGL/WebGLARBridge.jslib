mergeInto(LibraryManager.library, {
    
    // Check if WebXR is supported in the current browser
    CheckWebXRSupport: function() {
        if (navigator.xr !== undefined) {
            return navigator.xr.isSessionSupported('immersive-ar').then(function(supported) {
                return supported;
            }).catch(function() {
                return false;
            });
        }
        return false;
    },
    
    // Start a WebXR AR session
    StartWebXRSession: function() {
        if (window.arSession) {
            console.log("AR session already started");
            return;
        }
        
        if (navigator.xr) {
            const sessionInit = {
                requiredFeatures: ['hit-test'],
                optionalFeatures: ['dom-overlay'],
            };
            
            // If DOM overlay is supported, use it to show UI elements
            if (sessionInit.optionalFeatures.includes('dom-overlay')) {
                sessionInit.domOverlay = { root: document.getElementById('ar-overlay') };
            }
            
            navigator.xr.requestSession('immersive-ar', sessionInit)
                .then(function(session) {
                    window.arSession = session;
                    
                    // Setup session and notify Unity
                    setupWebXRSession(session);
                    
                    // Call Unity when session ends
                    session.addEventListener('end', function() {
                        window.arSession = null;
                        // Notify Unity that AR session has ended
                        unityInstance.SendMessage('WebGLARBridge', 'OnWebXRSessionEnded');
                    });
                    
                    // Notify Unity that AR session has started
                    unityInstance.SendMessage('WebGLARBridge', 'OnWebXRSessionStarted');
                })
                .catch(function(err) {
                    console.error("Error starting AR session: ", err);
                });
        } else {
            console.error("WebXR not supported in this browser");
        }
    },
    
    // Stop the current WebXR AR session
    StopWebXRSession: function() {
        if (window.arSession) {
            window.arSession.end();
        }
    },
    
    // Request hit test against real-world surfaces
    GetWebXRHitResults: function() {
        if (!window.arSession || !window.xrHitTestSource) {
            console.log("AR session or hit test source not available");
            return;
        }
        
        // The actual hit test is performed in the XR frame callback
        // See setupWebXRSession for implementation
        window.requestHitTest = true;
    }
});

// Helper function to setup WebXR session
function setupWebXRSession(session) {
    if (!session) return;
    
    // Create a WebGL layer for rendering
    const canvas = document.getElementById('unity-canvas');
    const gl = canvas.getContext('webgl', { xrCompatible: true });
    
    const xrGLLayer = new XRWebGLLayer(session, gl);
    session.updateRenderState({ baseLayer: xrGLLayer });
    
    // Setup reference space
    session.requestReferenceSpace('local')
        .then(function(refSpace) {
            window.xrRefSpace = refSpace;
            
            // Setup hit test source
            session.requestReferenceSpace('viewer')
                .then(function(viewerSpace) {
                    session.requestHitTestSource({ space: viewerSpace })
                        .then(function(hitTestSource) {
                            window.xrHitTestSource = hitTestSource;
                        })
                        .catch(function(err) {
                            console.error("Error creating hit test source: ", err);
                        });
                });
            
            // Start the XR animation loop
            session.requestAnimationFrame(onXRFrame);
        });
}

// XR animation frame callback
function onXRFrame(time, frame) {
    const session = frame.session;
    session.requestAnimationFrame(onXRFrame);
    
    if (!window.xrRefSpace) return;
    
    // Get viewer pose
    const pose = frame.getViewerPose(window.xrRefSpace);
    if (!pose) return;
    
    // Perform hit test if requested
    if (window.requestHitTest && window.xrHitTestSource) {
        const hitTestResults = frame.getHitTestResults(window.xrHitTestSource);
        if (hitTestResults.length > 0) {
            const hitPose = hitTestResults[0].getPose(window.xrRefSpace);
            if (hitPose) {
                // Convert hit position to JSON and send to Unity
                const hitPosition = {
                    x: hitPose.transform.position.x,
                    y: hitPose.transform.position.y,
                    z: hitPose.transform.position.z
                };
                
                const hitPositionJson = JSON.stringify(hitPosition);
                unityInstance.SendMessage('WebGLARBridge', 'OnWebXRHitResult', hitPositionJson);
            }
        }
        
        window.requestHitTest = false;
    }
}