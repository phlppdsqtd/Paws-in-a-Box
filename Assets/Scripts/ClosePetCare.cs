using UnityEngine;
using UnityEngine.SceneManagement;

public class ClosePetCare : MonoBehaviour
{
    public void GoBackToCafe()
    {
        // --- NEW: Wake the AR Cafe back up! ---
        if (ARPlaceCafe.Instance != null)
        {
            ARPlaceCafe.Instance.ResumeCafeSystems();
        }

        // This destroys Level2, automatically revealing the resumed Level1 right beneath it!
        SceneManager.UnloadSceneAsync("Level2");
    }
}