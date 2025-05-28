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
        if (!base.Interact(interactor))
        {
            return false;
        }

        switch (m_perkType)
        {
            case Perks.ExtraHealth:
            
                break;
            case Perks.FastHands:

                break;
            case Perks.Stamina:

                break;
            case Perks.DoubleDamage:

                break;
            case Perks.Revive:

                break;
            default:
                break;
        }

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
        base.ShowInteract(interactor, look);

        return true;
    }

    public override bool HideInteract(PlayerController interactor, bool look)
    {
        base.HideInteract(interactor, look);

        return true;
    }
}
