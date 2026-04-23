using UnityEngine;
using UnityEngine.SceneManagement;

public class PetClicker : MonoBehaviour
{
    // This built-in Unity method automatically runs when the mouse clicks
    // (or a finger taps) the Collider attached to this specific GameObject.
    private void OnMouseDown()
    {
        // --- NEW: Pause the 3D AR Cafe to save battery and performance ---
        if (ARPlaceCafe.Instance != null)
        {
            ARPlaceCafe.Instance.PauseCafeSystems();
        }
        
        // Load the Pet Care scene
        SceneManager.LoadScene("Level2", LoadSceneMode.Additive);
    }
}