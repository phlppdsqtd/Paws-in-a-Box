using UnityEngine;
using UnityEngine.UI;
using TMPro; // Needed to change the text

public class BuyPetButton : MonoBehaviour
{
    public int myPetIndex; 
    public int myCost;     
    public TMP_Text buttonText; // Drag your button's text object here!

    private ShopManager shopManager;
    private Button myButton;

    void Start()
    {
        shopManager = FindObjectOfType<ShopManager>();
        myButton = GetComponent<Button>();
        
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnButtonClicked);
        }
    }

    // OnEnable runs every single time the Shop UI is opened
    void OnEnable()
    {
        RefreshButtonUI();
    }

    public void RefreshButtonUI()
    {
        // Check if this specific pet is saved as unlocked
        if (PlayerPrefs.GetInt("PetUnlocked_" + myPetIndex, 0) == 1)
        {
            if (buttonText != null) buttonText.text = "N/A";
            // Note: We leave the button interactable so they can click it and see the "Already owned" pop-up!
        }
        else
        {
            if (buttonText != null) buttonText.text = "" + myCost;
        }
    }

    void OnButtonClicked()
    {
        if (shopManager != null)
        {
            // We pass 'this' so the ShopManager knows exactly which button to update
            shopManager.BuyPet(myPetIndex, myCost, this);
        }
    }
}