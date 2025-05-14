using UnityEngine;

public class InteractableDoor : InteractableCostPointsBase
{
    public Animation m_openDoor;

    public override bool Interact(PlayerController interactor)
    {
        if (!base.Interact(interactor))
        {
            return false;
        }
        m_openDoor.Play();

        return true;
    }
    public override bool ShowInteract(PlayerController interactor)
    {
        base.ShowInteract(interactor);

        return true;
    }

    public override bool HideInteract(PlayerController interactor)
    {
        base.HideInteract(interactor);

        return true;
    }
}
