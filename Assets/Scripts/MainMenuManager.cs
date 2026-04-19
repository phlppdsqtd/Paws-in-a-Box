using UnityEngine;
using UnityEngine.UI; // Required for interacting with Button components
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button loadGameButton;
    public Button startGameButton;
    public Button quitButton;

    [Header("UI Panels")]
    public GameObject overwriteWarningPanel;

    // We use a specific PlayerPrefs key to know if a save exists.
    // If your save system uses a different key, change "HasSaveData" to match yours!
    private string saveKey = "HasSaveData"; 

    void Start()
    {
        // 1. Ensure the warning panel is hidden when the menu loads
        overwriteWarningPanel.SetActive(false);

        // 2. Check if a save file exists
        // (Assuming you set this PlayerPref to 1 when you save the game)
        if (PlayerPrefs.HasKey(saveKey) && PlayerPrefs.GetInt(saveKey) == 1)
        {
            // Save exists! Make the button clickable.
            loadGameButton.interactable = true;
        }
        else
        {
            // No save. Gray out the button.
            loadGameButton.interactable = false;
        }
    }

    // --- BUTTON CLICK METHODS ---

    public void OnLoadGameClicked()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OnStartGameClicked()
    {
        // If they click Start, we check if they already have a cafe
        if (PlayerPrefs.HasKey(saveKey) && PlayerPrefs.GetInt(saveKey) == 1)
        {
            // Show the warning prompt!
            overwriteWarningPanel.SetActive(true);
        }
        else
        {
            // No save data to overwrite, just start a fresh game
            StartFreshGame();
        }
    }

    public void OnProceedOverwriteClicked()
    {
        // 1. Delete all saved data
        PlayerPrefs.DeleteAll(); 
        
        // 2. Start the fresh game
        StartFreshGame();
    }

    public void OnCancelOverwriteClicked()
    {
        // Hide the prompt and let them rethink their choices
        overwriteWarningPanel.SetActive(false);
    }

    public void OnQuitClicked()
        {
            Debug.Log("Quitting Game...");

            // If we are testing inside the Unity Editor, stop playing
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            // If we are playing the actual built game, close the app
            Application.Quit(); 
    #endif
        }

    // --- HELPER METHOD ---
    private void StartFreshGame()
    {
        // Create the flag so the Load button works next time they boot the app!
        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene("Level1");
    }
}