using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
   [SerializeField] private Transform mainCam;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite chasingMark;
    [SerializeField] private Sprite boundaryMark;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
}
