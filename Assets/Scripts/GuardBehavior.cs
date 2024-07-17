using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehavior : MonoBehaviour
{
    // Guard states
    public enum GuardState { Normal, Chasing, Boundary }

    // Current state of the guard
    public GuardState currentState = GuardState.Normal;

    // Patrol points for normal mode
    public Transform[] patrolPoints;

    // Recognition ranges and angles for different modes
    public float normalRecognitionRange;
    public float chasingRecognitionRange;
    public float boundaryRecognitionRange;
    public float recognitionAngle = 60f;

    // Duration the guard stays in Boundary mode
    public float boundaryDuration = 5f;

    // Alert range for notifying nearby guards
    public float alertRange;

    // Layers for detecting the player and obstacles
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public LayerMask enemyLayer;

    private int currentPatrolIndex = 0; // Current patrol point index
    private NavMeshAgent agent; // NavMeshAgent component for movement
    [SerializeField]private Transform player; // Reference to the player
    private float boundaryTimer = 0f; // Timer for Boundary mode

    [SerializeField]private BillBoard billBoard;

    void Start()
    {
        billBoard = GetComponentInChildren<BillBoard>();

        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        // Find the player in the scene
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Start patrolling
        GoToNextPatrolPoint();
    }

    void Update()
    {
        // Update behavior based on the current state
        switch (currentState)
        {
            case GuardState.Normal:
                
                Patrol(); // Perform patrol behavior
                CheckForPlayer(normalRecognitionRange); // Check for player in normal range
                break;
            case GuardState.Chasing:
                
                ChasePlayer(); // Perform chasing behavior
                break;
            case GuardState.Boundary:
                
                HandleBoundaryMode(); // Handle boundary mode behavior
                break;
        }
    }

    // Patrol between points
    void Patrol()
    {
        if (billBoard != null) billBoard.gameObject.SetActive(false);
        // If the agent is not moving or has reached the destination
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint(); // Move to the next patrol point
        }
    }

    // Move to the next patrol point
    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // Check for the player within a specified range and angle
    void CheckForPlayer(float range)
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, range, playerLayer);

        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;

            // Check if the player is within the recognition angle
            if (Vector3.Angle(transform.forward, dirToTarget) < recognitionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                // Check if there's a clear line of sight to the player
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleLayer))
                {
                    currentState = GuardState.Chasing;
                    return;
                }
            }
        }
    }

    // Chase the player
    void ChasePlayer()
    {
        billBoard.gameObject.SetActive(true);
        billBoard.SetChasingMark();

        agent.destination = player.position;

        // If the player moves out of chasing range, return to normal mode
        if (Vector3.Distance(transform.position, player.position) > chasingRecognitionRange)
        {
            currentState = GuardState.Normal;
            GoToNextPatrolPoint();
        }

        // Alert nearby guards
        AlertNearbyGuards();
    }

    void AlertNearbyGuards()
    {
        Collider[] guardsInRange = Physics.OverlapSphere(transform.position, alertRange, enemyLayer);
        foreach (Collider guard in guardsInRange)
        {
            GuardBehavior guardBehavior = guard.GetComponent<GuardBehavior>();
            Debug.Log(guard);
            if (guardBehavior != null && guardBehavior != this)
            {
                guardBehavior.EnterBoundaryMode();
            }
        }
    }

    // Handle boundary mode behavior
    void HandleBoundaryMode()
    {
        billBoard.gameObject.SetActive(true);
        billBoard.SetBoundaryMark();

        boundaryTimer += Time.deltaTime;

        // If the boundary mode timer expires, return to normal mode
        if (boundaryTimer >= boundaryDuration)
        {
            boundaryTimer = 0f;
            currentState = GuardState.Normal;
            GoToNextPatrolPoint();
        }
        else
        {
            CheckForPlayer(boundaryRecognitionRange); // Check for player in boundary range
        }
    }

    // Enter boundary mode and reset the timer
    public void EnterBoundaryMode()
    {
        if (currentState != GuardState.Chasing)
        {
            currentState = GuardState.Boundary;
            boundaryTimer = 0f;
        }
    }

    // Draw gizmos to visualize recognition ranges in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, normalRecognitionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chasingRecognitionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, boundaryRecognitionRange);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, alertRange);

        // Draw recognition angles
        DrawRecognitionAngleGizmo(normalRecognitionRange, Color.yellow);
        DrawRecognitionAngleGizmo(chasingRecognitionRange, Color.red);
        DrawRecognitionAngleGizmo(boundaryRecognitionRange, Color.blue);
    }

    void DrawRecognitionAngleGizmo(float range, Color color)
    {
        Gizmos.color = color;
        Vector3 leftBoundary = Quaternion.Euler(0, -recognitionAngle / 2, 0) * transform.forward * range;
        Vector3 rightBoundary = Quaternion.Euler(0, recognitionAngle / 2, 0) * transform.forward * range;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Approximate the arc by drawing multiple lines
        int segments = 20; // Number of segments for the arc
        float segmentAngle = recognitionAngle / segments;
        Vector3 previousPoint = transform.position + leftBoundary;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -recognitionAngle / 2 + segmentAngle * i;
            Vector3 nextPoint = transform.position + Quaternion.Euler(0, currentAngle, 0) * transform.forward * range;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}
