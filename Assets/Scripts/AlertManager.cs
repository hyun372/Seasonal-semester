using UnityEngine;
using UnityEngine.AI;

public class AlertManager : MonoBehaviour
{
    // Alert all guards and make them enter boundary mode
    public void AlertAllGuards(Vector3 alertPosition)
    {
        // Find all guards in the scene
        GuardBehavior[] allGuards = FindObjectsOfType<GuardBehavior>();

        foreach (GuardBehavior guard in allGuards)
        {
            guard.EnterBoundaryMode(); // Make each guard enter boundary mode

            // Optionally, move towards the alert position
            guard.GetComponent<NavMeshAgent>().destination = alertPosition;
        }
    }
}
