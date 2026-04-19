using UnityEngine;
using TMPro; 
using System; 

public class EconomyManager : MonoBehaviour
{
    [Header("Economy Settings")]
    public int currentCoins = 120; 
    public TMP_Text coinUIText; 

    [Header("Offline Earnings Settings")]
    public float coinsPerMinute = 0.5f; 
    public GameObject welcomeBackPanel;
    public TMP_Text offlineCoinsText; 

    // --- NEW: Keep track of coins waiting to be shown ---
    private int pendingOfflineCoins = 0;

    void Start()
    {
        currentCoins = PlayerPrefs.GetInt("SavedCoins", 120);
        UpdateCoinUI();
        CalculateOfflineEarnings();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        SaveCoins(); 
        UpdateCoinUI();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            SaveCoins(); 
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

    private void SaveCoins()
    {
        PlayerPrefs.SetInt("SavedCoins", currentCoins);
        PlayerPrefs.Save();
    }

    private void SaveCurrentTime()
    {
        PlayerPrefs.SetString("LastPlayedTime", DateTime.Now.ToString());
        PlayerPrefs.Save(); 
    }

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
                    // Add coins silently and set up the text, but DO NOT show the panel yet!
                    AddCoins(offlineCoinsEarned); 
                    pendingOfflineCoins = offlineCoinsEarned;

                    if (offlineCoinsText != null) 
                        offlineCoinsText.text = "" + offlineCoinsEarned;
                }
            }
        }
    }

    // --- NEW METHOD: Called by ARPlaceCafe once the cafe is placed ---
    public void ShowPendingWelcomePanel()
    {
        if (pendingOfflineCoins > 0 && welcomeBackPanel != null)
        {
            welcomeBackPanel.SetActive(true);
            pendingOfflineCoins = 0; // Reset it so it doesn't pop up again accidentally
        }
    }

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