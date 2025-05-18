using UnityEngine;

public interface IInteractable
{
    bool Interact(PlayerController interactor);
    bool ShowInteract(PlayerController interactor, bool looking);
    bool HideInteract(PlayerController interactor, bool looking);
}
