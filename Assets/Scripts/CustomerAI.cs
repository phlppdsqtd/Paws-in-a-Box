using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CustomerAI : MonoBehaviour
{
    private enum State { WalkingToTable, Sitting, Leaving }
    private State currentState;

    [Header("AI Settings")]
    public float sitDuration = 15f; 
    public int minPay = 3;
    public int maxPay = 5;

    [Header("Visual Effects")]
    public GameObject coinVFXPrefab; 
    public float coinSpawnHeight = 0.1f; 

    // --- Exit Fix Settings ---
    [Header("Exit Settings")]
    public float despawnDistance = 0.2f; 
    public float maxExitTime = 5f;      
    private float exitTimer = 0f;        
    // ------------------------------

    private NavMeshAgent agent;
    private EconomyManager economyManager;
    private Transform exitDoor;
    private TableData claimedTable; 
    
    // --- Animator Reference ---
    private Animator customerAnimator;

    private CustomerSpawner mySpawner;
    private int myPrefabIndex;

    // --- NEW: The Amnesia Fix Flag ---
    private bool needsPathRestore = false;

    public void SetupSpawner(CustomerSpawner spawner, int index)
    {
        mySpawner = spawner;
        myPrefabIndex = index;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        economyManager = FindObjectOfType<EconomyManager>();
        
        // Grab the Animator from the 3D model
        customerAnimator = GetComponentInChildren<Animator>();

        GameObject[] allTables = GameObject.FindGameObjectsWithTag("CafeTable");
        List<GameObject> freeTables = new List<GameObject>();

        foreach (GameObject t in allTables)
        {
            TableData data = t.GetComponent<TableData>();
            if (data != null && data.isOccupied == false)
            {
                freeTables.Add(t);
            }
        }

        if (freeTables.Count > 0)
        {
            int randomIndex = Random.Range(0, freeTables.Count);
            GameObject chosenTable = freeTables[randomIndex];
            
            claimedTable = chosenTable.GetComponent<TableData>();
            claimedTable.isOccupied = true; 

            agent.SetDestination(chosenTable.transform.position);
            currentState = State.WalkingToTable;
        }
        else
        {
            Debug.Log("Cafe is full! Customer is leaving.");
            FindExitAndLeave();
        }

        GameObject doorObj = GameObject.FindGameObjectWithTag("ExitDoor");
        if (doorObj != null) exitDoor = doorObj.transform;
    }

    // --- NEW: This runs automatically every time the customer wakes up from being paused ---
    private void OnEnable()
    {
        needsPathRestore = true; 
    }

    void Update()
    {
        // --- SAFETY SHIELD ---
        // If the agent is paused, missing, or hasn't stuck to the floor yet, skip this ENTIRE frame!
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        // --- NEW: The Amnesia Fix ---
        if (needsPathRestore)
        {
            needsPathRestore = false; // Turn the flag off so we only do this once per wake-up
            
            if (currentState == State.WalkingToTable && claimedTable != null)
            {
                agent.SetDestination(claimedTable.transform.position); // Remind them of the table!
            }
            else if (currentState == State.Leaving && exitDoor != null)
            {
                agent.SetDestination(exitDoor.position); // Remind them of the door!
            }
        }
        // -----------------------------

        // --- Automatically sync animation with NavMeshAgent movement speed ---
        if (customerAnimator != null)
        {
            // If the agent's velocity is greater than 0.1, it's moving!
            bool isMoving = agent.velocity.magnitude > 0.1f;
            customerAnimator.SetBool("isWalking", isMoving);
        }
        // ------------------------------------------------------------------------

        if (currentState == State.WalkingToTable)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                currentState = State.Sitting;
                
                // Randomly change direction when arriving at the table
                float randomYAngle = Random.Range(0f, 360f);
                transform.rotation = Quaternion.Euler(0f, randomYAngle, 0f);
            }
        }
        else if (currentState == State.Sitting)
        {
            sitDuration -= Time.deltaTime;

            if (sitDuration <= 0)
            {
                // 1. Give money to the cafe
                int payment = Random.Range(minPay, maxPay + 1); 
                if (economyManager != null) economyManager.AddCoins(payment);

                // Tell the UI Manager to pop up the text!
                if (UIPopupManager.Instance != null)
                {
                    UIPopupManager.Instance.ShowPayment(payment);
                }

                // 2. Spawn Coin VFX
                if (coinVFXPrefab != null)
                {
                    Vector3 spawnPos = transform.position + (Vector3.up * coinSpawnHeight);
                    Instantiate(coinVFXPrefab, spawnPos, Quaternion.identity);
                }

                // 3. Free up the table
                if (claimedTable != null)
                {
                    claimedTable.isOccupied = false;
                }

                // 4. Head to the exit
                FindExitAndLeave();
            }
        }
        else if (currentState == State.Leaving)
        {
            exitTimer += Time.deltaTime;

            if (Vector3.Distance(transform.position, agent.destination) < despawnDistance || exitTimer > maxExitTime)
            {
                DespawnRoutine();
            }
        }
    }

    private void FindExitAndLeave()
    {
        if (exitDoor != null)
        {
            agent.SetDestination(exitDoor.position);
        }
        
        exitTimer = 0f; 
        
        currentState = State.Leaving;
    }

    private void DespawnRoutine()
    {
        if (mySpawner != null)
        {
            mySpawner.CustomerLeft(myPrefabIndex);
        }
        
        Destroy(gameObject);
    }
}