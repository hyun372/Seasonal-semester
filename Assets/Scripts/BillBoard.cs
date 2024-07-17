using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
   [SerializeField] private Transform mainCam;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite chasingMark;
    [SerializeField] private Sprite boundaryMark;
    [SerializeField] private Sprite alertMoveMark;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(mainCam == null)
        {
            mainCam = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCam.rotation * Vector3.forward,
            mainCam.rotation * Vector3.up);
    }

    public void SetChasingMark()
    {
        spriteRenderer.sprite = chasingMark;
    }
    public void SetBoundaryMark()
    {
        spriteRenderer.sprite = boundaryMark;
    }
    public void SetAlertMoveMark()
    {
        spriteRenderer.sprite = alertMoveMark;
    }
}
