using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    public float openAngle = 90.0f;
    public float animationDuration = 1.0f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    //private float animationTime = 0.0f;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * openAngle);
    }

    public void Interact()
    {
        if (isOpen)
        {
            StartCoroutine(RotateDoor(closedRotation));
        }
        else
        {
            StartCoroutine(RotateDoor(openRotation));
        }
        isOpen = !isOpen;
    }

    IEnumerator RotateDoor(Quaternion targetRotation)
    {
        float startTime = Time.time;
        Quaternion initialRotation = transform.rotation;

        while (Time.time - startTime < animationDuration)
        {
            float t = (Time.time - startTime) / animationDuration;
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            yield return null;
        }
        transform.rotation = targetRotation;
    }
}
