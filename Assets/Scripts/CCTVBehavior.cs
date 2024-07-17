using UnityEngine;

public class CCTVBehavior : MonoBehaviour
{
    public float rotationSpeed = 30f; // Rotation speed of the CCTV
    public float recognitionRange = 15f; // Recognition range of the CCTV
    public float recognitionAngle = 90f; // Recognition angle of the CCTV
    public LayerMask playerLayer; // Layer mask to detect the player
    public LayerMask obstacleLayer; // Layer mask to detect obstacles
    public float alertRange = 20f; // Range within which guards are alerted
    public Transform[] guards; // List of guards to alert
    public LineRenderer lineRenderer; // LineRenderer component to draw the recognition scope

    private bool alarmTriggered = false; // Flag to check if the alarm has been triggered

    void Start()
    {
        // Initialize the LineRenderer to draw the recognition scope
        lineRenderer.positionCount = 31; // Number of points in the arc (increase for a smoother arc)
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        RotateCCTV(); // Rotate the CCTV
        CheckForPlayer(); // Check for the player
        DrawRecognitionScope(); // Draw the recognition scope
    }

    void RotateCCTV()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // Rotate the CCTV around the Y axis
    }

    void CheckForPlayer()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, recognitionRange, playerLayer);

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
                    TriggerAlarm(); // Trigger the alarm if the player is detected
                    return;
                }
            }
        }
    }

    void TriggerAlarm()
    {
        if (!alarmTriggered)
        {
            alarmTriggered = true;
            Debug.Log("Alarm triggered!");
            AlertNearbyGuards(); // Alert nearby guards
        }
    }

    void AlertNearbyGuards()
    {
        Collider[] guardsInRange = Physics.OverlapSphere(transform.position, alertRange, obstacleLayer);

        foreach (Collider guard in guardsInRange)
        {
            GuardBehavior guardBehavior = guard.GetComponent<GuardBehavior>();
            if (guardBehavior != null)
            {
                guardBehavior.EnterBoundaryMode(); // Set guards to boundary mode
            }
        }
    }

    void DrawRecognitionScope()
    {
        float halfAngle = recognitionAngle / 2;
        float stepAngle = recognitionAngle / (lineRenderer.positionCount - 1);

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float currentAngle = -halfAngle + stepAngle * i;
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = rotation * transform.forward;
            Vector3 position = transform.position + direction * recognitionRange;

            lineRenderer.SetPosition(i, position);
        }
    }

    // Draw gizmos to visualize recognition ranges and angles in the editor
    void OnDrawGizmosSelected()
    {
        // Draw recognition range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, recognitionRange);

        // Draw recognition angle
        DrawRecognitionAngleGizmo(recognitionRange, Color.red);

        // Draw alert range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, alertRange);
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
