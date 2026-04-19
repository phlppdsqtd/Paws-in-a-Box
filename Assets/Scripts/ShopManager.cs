using UnityEngine;
using TMPro; // Need this for the pop-up text!

public class ShopManager : MonoBehaviour
{
    [Header("Manager References")]
    public EconomyManager economyManager;
    public GameObject shopPanel;

    [Header("Pop-up UI Settings")]
    public GameObject notificationPanel;
    public TMP_Text notificationText;

    [HideInInspector] 
    public CafeSetup activeCafe; 

    public void BuyPet(int petIndex, int cost, BuyPetButton clickedButton)
    {
        // 1. Check if they ALREADY own this pet!
        // We check PlayerPrefs. If "PetUnlocked_1" exists and equals 1, they own it.
        if (PlayerPrefs.GetInt("PetUnlocked_" + petIndex, 0) == 1)
        {
        ShowNotification("This little one is already part of the family! Head to the playpen to say hi.");
            return; // Stop the code here
        }

        // 2. Check if they have enough money
        if (economyManager == null) return;

        bool purchaseSuccessful = economyManager.SpendCoins(cost);

        if (purchaseSuccessful)
        {
            // 3. Save the purchase to the phone!
            PlayerPrefs.SetInt("PetUnlocked_" + petIndex, 1);
            PlayerPrefs.Save();

            // 4. Update the button text to say "Owned"
            clickedButton.RefreshButtonUI();

            // 5. Turn the pet on in the AR Cafe (if it's placed)
            if (activeCafe != null)
            {
                activeCafe.TurnOnPet(petIndex);
            }
            
            CloseShop(); // Optional: You can remove this if you want them to stay in the shop after buying
        }
        else
        {
            // 6. Not enough money!
            ShowNotification("Not quite enough coins yet! Keep the cafe bustling to bring this friend home.");
        }
    }

    // --- POP-UP LOGIC ---
    public void ShowNotification(string message)
    {
        if (notificationText != null) notificationText.text = message;
        if (notificationPanel != null) notificationPanel.SetActive(true);
    }

    public void CloseNotification()
    {
        if (notificationPanel != null) notificationPanel.SetActive(false);
    }

    // --- SHOP VISIBILITY ---
    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
    }
    
    public void OpenShop()
    {
        if (shopPanel != null) shopPanel.SetActive(true);
    }
}