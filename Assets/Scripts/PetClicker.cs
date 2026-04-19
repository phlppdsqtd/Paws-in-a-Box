using UnityEngine;
using UnityEngine.SceneManagement;

public class PetClicker : MonoBehaviour
{
    // This built-in Unity method automatically runs when the mouse clicks
    // (or a finger taps) the Collider attached to this specific GameObject.
    private void OnMouseDown()
    {
        //Debug.Log("Pet clicked! Traveling to Pet Care...");
        
        // Load the Pet Care scene
        SceneManager.LoadScene("Level2", LoadSceneMode.Additive);
    }
}