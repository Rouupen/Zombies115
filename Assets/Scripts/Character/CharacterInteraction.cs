using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteraction : MonoBehaviour
{
    public float m_interactionDistance = 2f;

    private IInteractable _currentInteracting;
    private IInteractable _triggerInteracting;

    private void Start()
    {
        GameManager.GetInstance().m_inputManager.m_interact.started += TryInteract;
    }

    void Update()
    {
        Ray ray = new Ray(GameManager.GetInstance().m_playerController.m_characterLook.transform.position, GameManager.GetInstance().m_playerController.m_characterLook.transform.forward);
        int mask = ~0;

        if (Physics.Raycast(ray, out RaycastHit hit, m_interactionDistance, mask, QueryTriggerInteraction.Ignore) && hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            if (interactable != null && _currentInteracting != interactable)
            {
                //TEMP
                interactable.ShowInteract(GameManager.GetInstance().m_playerController, true);
                _currentInteracting = interactable;
            }

        }
        else
        {
            if (_currentInteracting != null)
            {
                _currentInteracting.HideInteract(GameManager.GetInstance().m_playerController, true);
                _currentInteracting = null;
            }
        }
    }

    void TryInteract(InputAction.CallbackContext context)
    {
        if (_triggerInteracting != null)
        {
            _triggerInteracting.Interact(GameManager.GetInstance().m_playerController);
            return;
        }
        Ray ray = new Ray(GameManager.GetInstance().m_playerController.m_characterLook.transform.position, GameManager.GetInstance().m_playerController.m_characterLook.transform.forward);
        int mask = ~0;
        if (Physics.Raycast(ray, out RaycastHit hit, m_interactionDistance, mask, QueryTriggerInteraction.Ignore))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                //TEMP
                interactable.Interact(GameManager.GetInstance().m_playerController);
            }
        }
    }


    public void AddInteract(IInteractable interact)
    {
        _triggerInteracting = interact;
    }
    public void RemoveInteract(IInteractable interact)
    {
        _triggerInteracting = null;
    }
}
