using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Header("Core Managers")]
    public GameObject soundManagerPrefab;

    void Awake()
    {
        // If the SoundManager hasn't been created yet, spawn it!
        if (SoundManager.instance == null)
        {
            Instantiate(soundManagerPrefab);
        }
    }
}