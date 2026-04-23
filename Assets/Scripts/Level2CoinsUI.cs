using UnityEngine;
using TMPro;

public class Level2CoinsUI : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    void Update()
    {
        // Instantly grabs the coins from Level 1's manager!
        if (EconomyManager.Instance != null)
        {
            coinsText.text = EconomyManager.Instance.currentCoins.ToString();
        }
    }
}