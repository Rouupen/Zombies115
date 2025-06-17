using UnityEngine;

public enum Perks
{
    ExtraHealth,
    FastHands,
    Stamina,
    DoubleDamage,
    Revive
}

public class InteractablePerk : InteractableCostPointsBase
{
    public Perks m_perkType;
    public override bool Interact(PlayerController interactor)
    {

        if (interactor.m_characterPerks.m_activePerks.Contains(m_perkType))
        {
            return false;
        }
        if (!base.Interact(interactor))
        {
            return false;
        }
        interactor.m_characterPerks.SetPerk(m_perkType);
        SetActive(true);
        return true;
    }
    public override bool Unlock()
    {
        if (!base.Unlock())
        {
            return false;
        }

        return true;
    }

    public override bool ShowInteract(PlayerController interactor, bool look)
    {
        if (interactor.m_characterPerks.m_activePerks.Contains(m_perkType))
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
