using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 2f;
    public string interactableTag = "Interactable"; // ��ȣ�ۿ� ������ ������Ʈ�� �±�

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, interactionRange))
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.CompareTag(interactableTag))
                {
                    IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact();
                    }
                }
            }
        }
    }
}
