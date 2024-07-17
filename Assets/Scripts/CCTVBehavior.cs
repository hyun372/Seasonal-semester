using UnityEngine;

public class CCTVBehavior : MonoBehaviour
{
    public float rotationSpeed = 30f; // Rotation speed of the CCTV
    public float rotateAngle = 120f; // Rotation angle of the CCTV
    private bool rotateDirCheck;
    private float rotateChekcer;
    public float recognitionRange = 15f; // Recognition range of the CCTV
    public float recognitionAngle = 90f; // Recognition angle of the CCTV

    public LayerMask playerLayer; // Layer mask to detect the player
    public LayerMask obstacleLayer; // Layer mask to detect obstacles
    public LayerMask enemyLayer; // Layer mask to detect enemies

    public float alertRange = 20f; // Range within which guards are alerted

    private bool alarmTriggered = false; // Flag to check if the alarm has been triggered
    private float alarmTimer = 0f; // Timer to reset the alarm
    [SerializeField] private Light recognitionLight;

    void Update()
    {
        RotateCCTV(); // Rotate the CCTV
        CheckForPlayer(); // Check for the player

        if (alarmTriggered)
        {
            recognitionLight.color = Color.red;
            alarmTimer += Time.deltaTime;
            if (alarmTimer >= 1f)
            {
                alarmTriggered = false;
                alarmTimer = 0f;
            }
        }
        else
        {
            recognitionLight.color = Color.yellow;
        }
    }

    void RotateCCTV()
    {
        Quaternion currentRotation = transform.rotation;
        Vector3 currentEulerAngles = currentRotation.eulerAngles;

        if (rotateChekcer >= rotateAngle / 2)
        {
            rotateDirCheck = false;
        }
        else if (rotateChekcer <= -rotateAngle / 2)
        {
            rotateDirCheck = true;
        }

        if (rotateDirCheck)
        {
            currentEulerAngles.y += rotationSpeed * Time.deltaTime;
            rotateChekcer += rotationSpeed * Time.deltaTime;
        }
        else
        {
            currentEulerAngles.y -= rotationSpeed * Time.deltaTime;
            rotateChekcer -= rotationSpeed * Time.deltaTime;
        }

        Quaternion newRotation = Quaternion.Euler(currentEulerAngles);

        transform.rotation = newRotation;
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
            BoundaryAllGuard(); // Set all guards to boundary mode
            AlertNearbyGuards(); // Alert nearby guards
        }
    }

    void AlertNearbyGuards()
    {
        Collider[] guardsInRange = Physics.OverlapSphere(transform.position, alertRange, enemyLayer);

        foreach (Collider guard in guardsInRange)
        {
            GuardBehavior guardBehavior = guard.GetComponent<GuardBehavior>();
            if (guardBehavior != null)
            {
                guardBehavior.EnterAlertMoveMode(this.transform); // Set guards to boundary mode
            }
        }
    }

    void BoundaryAllGuard(){
        Collider[] guardsInRange = Physics.OverlapSphere(transform.position, 60, enemyLayer);


        foreach (Collider guard in guardsInRange)
        {
            GuardBehavior guardBehavior = guard.GetComponent<GuardBehavior>();
            if (guardBehavior != null)
            {
                guardBehavior.EnterBoundaryMode();
            }
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
