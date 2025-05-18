using UnityEngine;

public class InteractableBuyWeapon : InteractableCostPointsBase
{
    public int m_weaponID;
    public override bool Interact(PlayerController interactor)
    {
        if (interactor.HaveWeaponOnInventory(m_weaponID))
        {
            return false;
        }
        if (!base.Interact(interactor))
        {
            return false;
        }
        interactor.ChangeSlotWeapon(m_weaponID);
        SetActive(true);
        return true;
    }
    public override bool ShowInteract(PlayerController interactor, bool look)
    {
        if (interactor.HaveWeaponOnInventory(m_weaponID))
        {
            return false;
        }
        if (interactor.HaveWeaponOnInventory(m_weaponID))
        {
            return false;
        }
        base.ShowInteract(interactor, look);

        return true;
    }

    public override bool HideInteract(PlayerController interactor, bool look)
    {
        base.HideInteract(interactor, look);

        return true;
    }
}
