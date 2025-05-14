using UnityEngine;

public interface IInteractable
{
    bool Interact(PlayerController interactor);
    bool ShowInteract(PlayerController interactor);
    bool HideInteract(PlayerController interactor);
}
