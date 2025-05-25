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
        if (_areaController != null)
        {
            _areaController.UnlockArea();
        }
        m_openDoor.Play();

        return true;
    }
    public override bool Unlock()
    {
        if (!base.Unlock())
        {
            return false;
        }

        m_openDoor.Play();

        return true;
    }

    public override bool ShowInteract(PlayerController interactor, bool look)
    {
        base.ShowInteract(interactor, look);

        return true;
    }

    public override bool HideInteract(PlayerController interactor, bool look)
    {
        base.HideInteract(interactor, look);

        return true;
    }
}
