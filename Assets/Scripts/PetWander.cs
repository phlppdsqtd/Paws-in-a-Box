using UnityEngine;

public class PetWander : MonoBehaviour
{
    [Header("Movement Area")]
    public float wanderRadius = 1.5f; 

    [Header("Speeds & Timing")]
    public float moveSpeed = 0.5f;
    public float turnSpeed = 5f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 6f;

    private Vector3 startLocalPosition;
    private Vector3 targetLocalPosition;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    
    // --- NEW: The Animator Reference ---
    private Animator petAnimator;

    void Start()
    {
        startLocalPosition = transform.localPosition;
        
        // This automatically finds the Animator on the child mesh!
        petAnimator = GetComponentInChildren<Animator>();
        
        PickNewWaypoint();
    }

    void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                PickNewWaypoint();
            }
            return;
        }

        Vector3 direction = (targetLocalPosition - transform.localPosition).normalized;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPosition, moveSpeed * Time.deltaTime);

        // --- NEW: Tell the Animator we are walking! ---
        if (petAnimator != null)
        {
            petAnimator.SetBool("isWalking", true);
        }

        if (Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.05f)
        {
            isWaiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
            
            // --- NEW: Tell the Animator to stop walking! ---
            if (petAnimator != null)
            {
                petAnimator.SetBool("isWalking", false);
            }
        }
    }

    private void PickNewWaypoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetLocalPosition = startLocalPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying && transform.parent != null 
            ? transform.parent.TransformPoint(startLocalPosition) 
            : transform.position;
            
        Gizmos.DrawWireSphere(center, wanderRadius);
    }
}