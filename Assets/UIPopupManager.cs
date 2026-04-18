using UnityEngine;

public class UIPopupManager : MonoBehaviour
{
    // This creates a Singleton, making it easy for any script to find it instantly!
    public static UIPopupManager Instance;

    public GameObject popupPrefab;
    public Transform spawnAnchor;

    void Awake()
    {
        Instance = this;
    }

    public void ShowPayment(int amount)
    {
        if (popupPrefab != null && spawnAnchor != null)
        {
            // Spawn the text inside the Anchor
            GameObject popup = Instantiate(popupPrefab, spawnAnchor.position, Quaternion.identity, spawnAnchor);
            
            // Pass the payment number to the script
            UIPaymentPopup script = popup.GetComponent<UIPaymentPopup>();
            if (script != null)
            {
                script.Setup(amount);
            }
        }
    }
}