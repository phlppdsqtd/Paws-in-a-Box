using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))] // Added to guarantee the script finds the Plane Manager
public class ARPlaceCafe : MonoBehaviour
{
    [Header("Assign your Cafe Prefab here:")]
    public GameObject cafePrefab;
    
    private GameObject spawnedCafe;
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager; // Added reference for the planes
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>(); // Grab the Plane Manager component
    }

    void Update()
    {
        // SAFETY CHECK: If the cafe is already spawned, do nothing.
        if (spawnedCafe != null) return;

        // Check for BOTH a screen tap (Mobile) OR a mouse click (Fallback/Editor)
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            // Determine the exact screen coordinates of the tap/click
            Vector2 screenPosition;
            if (Input.touchCount > 0)
                screenPosition = Input.GetTouch(0).position;
            else
                screenPosition = Input.mousePosition;

            // TrackableType.Planes detects hits anywhere on the detected horizontal plane
            if (raycastManager.Raycast(screenPosition, hits, TrackableType.Planes))
            {
                // Get the exact 3D position and rotation of where the tap hit the floor
                Pose hitPose = hits[0].pose;

                // Spawn the cafe prefab at that exact position!
                spawnedCafe = Instantiate(cafePrefab, hitPose.position, hitPose.rotation);
                
                // Vibrate the phone to prove the touch and spawn worked!
                Handheld.Vibrate();

                // Call our new function to clean up the yellow planes
                DisableARPlanes();
            }
        }
    }

    // --- NEW FEATURE: HIDE THE PLANES ---
    private void DisableARPlanes()
    {
        // 1. Turn off the manager so it stops scanning your room for new floors
        planeManager.enabled = false;

        // 2. Loop through every single yellow plane it already created and turn them invisible
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }
}