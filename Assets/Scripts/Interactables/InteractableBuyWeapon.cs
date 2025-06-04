using UnityEngine;

public class InteractableBuyWeapon : InteractableCostPointsBase
{
    public int m_weaponID;
    public int m_buyAmmoPoints = 100;
    public override bool Interact(PlayerController interactor)
    {
        if (interactor.GetCurrentWeapon().GetWeaponID() == m_weaponID && interactor.GetCurrentWeapon().GetReserveAmmo() != GameManager.GetInstance().m_weaponsInGame.GetWeaponData(m_weaponID).m_weaponStats.m_totalAmmo)
        {
            interactor.GetCurrentWeapon().SetReserveAmmo(GameManager.GetInstance().m_weaponsInGame.GetWeaponData(m_weaponID).m_weaponStats.m_totalAmmo);
            interactor.m_UIController.m_interactTextController.RemoveText();
            return true;
        }
        else if (interactor.HaveWeaponOnInventory(m_weaponID))
        {
            return true;
        }
        if (!base.Interact(interactor))
        {
            return true;
        }
        interactor.ChangeSlotWeapon(m_weaponID);
        SetActive(true);
        return true;
    }
    public override bool ShowInteract(PlayerController interactor, bool look)
    {
        if (interactor.GetCurrentWeapon().GetWeaponID() == m_weaponID && interactor.GetCurrentWeapon().GetReserveAmmo() != GameManager.GetInstance().m_weaponsInGame.GetWeaponData(m_weaponID).m_weaponStats.m_totalAmmo) //Bug when change weapon
        {
            string text = "Buy full ammo" + $"\n Cost: {m_buyAmmoPoints}";
            interactor.m_UIController.m_interactTextController.SetText(text);
            return true;
        }
        else if (interactor.HaveWeaponOnInventory(m_weaponID))
        {
            return true;
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
