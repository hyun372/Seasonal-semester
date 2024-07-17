using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVRoateAngleRange : MonoBehaviour
{
    [SerializeField] private float rotateAngle;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Vector3 leftBoundary = Quaternion.Euler(0, -rotateAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, rotateAngle / 2, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary.normalized * 10f);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary.normalized * 10f);

    }
    
}
