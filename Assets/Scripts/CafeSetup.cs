using UnityEngine;

public class CafeSetup : MonoBehaviour
{
    [Header("All Pets in Cafe")]
    public GameObject[] allPets; 

    void Start()
    {
        ShopManager shopManager = FindObjectOfType<ShopManager>();
        
        if (shopManager != null)
        {
            shopManager.activeCafe = this;
        }

        // --- NEW: LOAD SAVED PETS ---
        // We start the loop at 1 (skipping 0) because 0 is the Dog, which is always free/on!
        for (int i = 1; i < allPets.Length; i++)
        {
            // If the save file says they own it, turn it on instantly
            if (PlayerPrefs.GetInt("PetUnlocked_" + i, 0) == 1)
            {
                TurnOnPet(i);
            }
        }
    }

    public void TurnOnPet(int index)
    {
        if (index >= 0 && index < allPets.Length)
        {
            if (allPets[index] != null)
            {
                allPets[index].SetActive(true); 
            }
        }
    }
}