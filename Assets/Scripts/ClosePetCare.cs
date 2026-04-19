using UnityEngine;
using UnityEngine.SceneManagement;

public class ClosePetCare : MonoBehaviour
{
    public void GoBackToCafe()
    {
        // This destroys Level2, automatically revealing the paused Level1 right beneath it!
        SceneManager.UnloadSceneAsync("Level2");
    }
}