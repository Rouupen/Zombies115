using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteraction : MonoBehaviour
{
    public float m_interactionDistance = 2f;

    private IInteractable _currentInteracting;

    private void Start()
    {
        GameManager.GetInstance().m_inputManager.m_interact.started += TryInteract;
    }

    void Update()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, m_interactionDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && _currentInteracting != interactable)
            {
                //TEMP
                interactable.ShowInteract(GameManager.GetInstance().m_playerController);
                _currentInteracting = interactable;
            }

        }
        else
        {
            if (_currentInteracting != null)
            {
                _currentInteracting.HideInteract(GameManager.GetInstance().m_playerController);
                _currentInteracting = null;
            }
        }
    }

    void TryInteract(InputAction.CallbackContext context)
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, m_interactionDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                //TEMP
                interactable.Interact(GameManager.GetInstance().m_playerController);
            }
        }
    }
}
