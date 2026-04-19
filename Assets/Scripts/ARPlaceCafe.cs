using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))] 
public class ARPlaceCafe : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject cafePrefab;
    
    [Tooltip("The glowing square or ghost object that shows where the cafe will go.")]
    public GameObject reticlePrefab;

    [Header("Grid Snapping Settings")]
    public float gridSize = 1.0f; 

    [Header("Placement UI Elements")]
    public GameObject instructionText; 
    public EconomyManager economyManager;

    // --- NEW: References for your Game UI ---
    [Header("Game UI Elements")]
    public GameObject quitButton;
    public GameObject coinsUI;
    public GameObject shopButton;

    private GameObject spawnedCafe;
    private GameObject spawnedReticle; 
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager; 
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>(); 
    }

    void Start()
    {
        // --- NEW: Hide the game UI the moment the game loads ---
        ToggleGameUI(false);

        // Instantiate the reticle right away, but keep it hidden until we find a floor
        if (reticlePrefab != null)
        {
            spawnedReticle = Instantiate(reticlePrefab);
            spawnedReticle.SetActive(false);
        }

        // Editor testing check
        if (spawnedCafe == null)
        {
            spawnedCafe = GameObject.FindGameObjectWithTag("Cafe");
            if (spawnedCafe != null)
            {
                if (instructionText != null) instructionText.SetActive(false);
                if (spawnedReticle != null) spawnedReticle.SetActive(false); 
                DisableARPlanes(); 
                
                // --- NEW: Show UI if testing in editor ---
                ToggleGameUI(true);

                if (economyManager != null) economyManager.ShowPendingWelcomePanel();
            }
        }
    }

    void Update()
    {
        // If the cafe is already built, do nothing!
        if (spawnedCafe != null) return;

        // 1. Always shoot a laser exactly from the dead center of the phone screen
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
        {
            // We found the floor! Get the position.
            Pose hitPose = hits[0].pose;

            // Apply our Grid Snapping Math
            Vector3 snappedPosition = hitPose.position;
            snappedPosition.x = Mathf.Round(snappedPosition.x / gridSize) * gridSize;
            snappedPosition.z = Mathf.Round(snappedPosition.z / gridSize) * gridSize;
            Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);

            // Turn on the reticle and move it to the snapped grid space
            if (spawnedReticle != null)
            {
                spawnedReticle.SetActive(true);
                spawnedReticle.transform.position = snappedPosition;
                spawnedReticle.transform.rotation = fixedRotation;
            }

            // --- PLACEMENT LOGIC ---
            // If the user taps anywhere on the screen while the reticle is showing...
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                // Spawn the cafe EXACTLY where the reticle currently is!
                spawnedCafe = Instantiate(cafePrefab, spawnedReticle.transform.position, spawnedReticle.transform.rotation);
                
                // Cleanup UI and Reticle
                if (instructionText != null) instructionText.SetActive(false);
                if (spawnedReticle != null) spawnedReticle.SetActive(false); // Turn off reticle forever

                Handheld.Vibrate();
                DisableARPlanes();

                // --- NEW: Show all the main game UI now that the cafe is placed! ---
                ToggleGameUI(true);

                if (economyManager != null) economyManager.ShowPendingWelcomePanel();
            }
        }
        else
        {
            // If the camera is pointing at a wall or somewhere with no floor, hide the reticle!
            if (spawnedReticle != null)
            {
                spawnedReticle.SetActive(false);
            }
        }
    }

    private void DisableARPlanes()
    {
        planeManager.enabled = false;
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }

    // --- NEW FEATURE: Helper method to easily turn game UI on or off ---
    private void ToggleGameUI(bool isVisible)
    {
        if (quitButton != null) quitButton.SetActive(isVisible);
        if (coinsUI != null) coinsUI.SetActive(isVisible);
        if (shopButton != null) shopButton.SetActive(isVisible);
    }
}