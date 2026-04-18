using UnityEngine;
using TMPro; 
using System; 

public class EconomyManager : MonoBehaviour
{
    [Header("Economy Settings")]
    public int currentCoins = 120; // Starting amount for brand new players
    public TMP_Text coinUIText; 

    [Header("Offline Earnings Settings")]
    public float coinsPerMinute = 0.5f; 
    public GameObject welcomeBackPanel;
    public TMP_Text offlineCoinsText; 

    void Start()
    {
        // 1. LOAD COINS: Check if we have saved coins. If not, default to 120.
        currentCoins = PlayerPrefs.GetInt("SavedCoins", 120);
        
        UpdateCoinUI();
        CalculateOfflineEarnings();
    }

    // --- YOUR NORMAL ECONOMY LOGIC ---
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        SaveCoins(); // Save immediately after earning
        UpdateCoinUI();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            SaveCoins(); // Save immediately after spending
            UpdateCoinUI();
            return true; 
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false; 
        }
    }

    private void UpdateCoinUI()
    {
        if (coinUIText != null) coinUIText.text = "" + currentCoins;
    }

    // --- SAVING LOGIC ---
    private void SaveCoins()
    {
        // Save the current bank balance
        PlayerPrefs.SetInt("SavedCoins", currentCoins);
        PlayerPrefs.Save();
    }

    private void SaveCurrentTime()
    {
        // Save the exact current date and time
        PlayerPrefs.SetString("LastPlayedTime", DateTime.Now.ToString());
        PlayerPrefs.Save(); 
    }

    // --- OFFLINE MATH LOGIC ---
    private void CalculateOfflineEarnings()
    {
        if (PlayerPrefs.HasKey("LastPlayedTime"))
        {
            string savedTimeString = PlayerPrefs.GetString("LastPlayedTime");
            DateTime lastPlayedTime;
            
            if (DateTime.TryParse(savedTimeString, out lastPlayedTime))
            {
                TimeSpan timeAway = DateTime.Now - lastPlayedTime;

                int offlineCoinsEarned = Mathf.CeilToInt((float)timeAway.TotalMinutes * coinsPerMinute);

                if (offlineCoinsEarned > 0)
                {
                    AddCoins(offlineCoinsEarned); // This will automatically trigger SaveCoins() now!
                    
                    if (offlineCoinsText != null) 
                        offlineCoinsText.text = "" + offlineCoinsEarned;
                    
                    if (welcomeBackPanel != null) 
                        welcomeBackPanel.SetActive(true);
                }
            }
        }
    }

    // --- APP STATE TRIGGERS ---
    void OnApplicationPause(bool isPaused)
    {
        if (isPaused) SaveCurrentTime();
    }

    void OnApplicationQuit()
    {
        SaveCurrentTime();
    }

    public void CloseWelcomePanel()
    {
        if (welcomeBackPanel != null) welcomeBackPanel.SetActive(false);
    }
}