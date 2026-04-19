using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] customerPrefabs; 
    public Transform spawnPoint; 

    [Header("Capacity Settings")]
    public int maxCustomers = 5;
    private int currentCustomers = 0;
    
    // The "Guest List" - keeps track of which prefab indices are currently inside
    private List<int> activeCustomerIndices = new List<int>();
    private int lastSpawnedIndex = -1;

    [Header("Timer Settings")]
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f; // Updated to 10
    
    private float timer;

    void Start()
    {
        SetRandomTimer();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnCustomer();
            SetRandomTimer(); 
        }
    }

    void SpawnCustomer()
    {
        if (customerPrefabs.Length == 0 || spawnPoint == null) return;

        // RULE 1: Max Capacity Check
        if (currentCustomers >= maxCustomers) return;

        // Figure out which prefabs are allowed to spawn right now
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < customerPrefabs.Length; i++)
        {
            // RULE 2: Is this specific model already inside? Skip it.
            if (activeCustomerIndices.Contains(i)) continue;

            // RULE 3: Was this model the exact last one to spawn? Skip it to prevent back-to-back spawns.
            // (We only enforce this if you have more than 2 prefabs, otherwise it might get stuck).
            if (i == lastSpawnedIndex && customerPrefabs.Length > 2) continue;

            availableIndices.Add(i);
        }

        // If no models pass the rules (e.g., they are all currently inside), cancel spawn and try again next timer
        if (availableIndices.Count == 0) return;

        // Pick a random allowed customer
        int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];

        // CREATE THE CUSTOMER!
        GameObject newCustomer = Instantiate(customerPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);

        // Update the guest list
        currentCustomers++;
        activeCustomerIndices.Add(randomIndex);
        lastSpawnedIndex = randomIndex;

        // Give the customer a reference to this Spawner and its ID so it can report back when leaving
        CustomerAI ai = newCustomer.GetComponent<CustomerAI>();
        if (ai != null)
        {
            ai.SetupSpawner(this, randomIndex);
        }
    }

    void SetRandomTimer()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    // The CustomerAI will call this function right before it destroys itself
    public void CustomerLeft(int prefabIndex)
    {
        currentCustomers--;
        activeCustomerIndices.Remove(prefabIndex);
    }
}