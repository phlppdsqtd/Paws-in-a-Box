using UnityEngine;
using UnityEngine.SceneManagement; // We must add this line to unlock Scene changing!

public class SceneController : MonoBehaviour
{
    // This method MUST be "public" so the button can see it
    public void LoadMainMenu()
    {
        // Make sure the spelling matches your scene name exactly!
        SceneManager.LoadScene("_MainMenu");
    }
}