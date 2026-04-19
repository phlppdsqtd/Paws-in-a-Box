using UnityEngine;

public class FloatingCoin : MonoBehaviour
{
    [Header("Animation Settings")]
    public float moveSpeed = 1.0f; // How fast it floats up
    public float lifetime = 1.5f;  // How long before it vanishes

    private SpriteRenderer spriteRenderer;
    private float fadeTimer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        fadeTimer = lifetime;

        // Tell Unity to automatically delete this coin after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 1. Move the coin smoothly upward
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);

        // 2. Always face the AR Camera (Billboarding)
        if (Camera.main != null)
        {
            // Make the coin look at the camera, then flip it so it's not backwards
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0); 
        }

        // 3. Fade out the alpha (transparency) smoothly over time
        if (spriteRenderer != null)
        {
            fadeTimer -= Time.deltaTime;
            // Lerp calculates the percentage of transparency based on remaining time
            float alpha = Mathf.Lerp(0, 1, fadeTimer / lifetime);
            
            Color newColor = spriteRenderer.color;
            newColor.a = alpha;
            spriteRenderer.color = newColor;
        }
    }
}