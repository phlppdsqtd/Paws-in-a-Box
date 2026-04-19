using UnityEngine;
using TMPro; // Needed for UI text

public class UIPaymentPopup : MonoBehaviour
{
    // UI uses pixels, not meters, so this number needs to be higher!
    public float floatSpeed = 50f; 
    public float fadeSpeed = 1f;

    private TextMeshProUGUI textMesh; // Notice this is UGUI for Canvas text!
    private CanvasGroup canvasGroup;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Destroy this UI element after 2 seconds so they don't pile up
        Destroy(gameObject, 2f); 
    }

    void Update()
    {
        // Float upwards
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out opacity
        if (canvasGroup != null)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
        }
    }

    public void Setup(int amount)
    {
        if (textMesh != null)
        {
            textMesh.text = "+" + amount.ToString();
        }
    }
}