using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PerksData
{
    public float m_extraHealth;
    public float m_fastHandsDivider;
    public float m_extraStamina;
    public float m_doubleDamageMulti;
    public bool m_revive;

    public PerksData(int empty)
    {
        m_extraHealth = 0;
        m_fastHandsDivider = 1;
        m_extraStamina = 0;
        m_doubleDamageMulti = 1;
        m_revive = false;
    }

}
public class CharacterPerks : MonoBehaviour
{
    public PerksData m_perksData;
    public List<Perks> m_activePerks;
    private PlayerController _playerController;

    private void Start()
    {
        m_perksData = new PerksData(0);
        m_activePerks = new List<Perks>();
        _playerController = GameManager.GetInstance().m_playerController;
    }

    public void SetPerk(Perks perk)
    {
        if (m_activePerks.Contains(perk))
        {
            return;
        }

        m_activePerks.Add(perk);

        switch (perk)
        {
            case Perks.ExtraHealth:
                _playerController.m_characterHealth.SetExtraHealth(GameManager.GetInstance().m_gameValues.m_perksData.m_extraHealth);
                break;
            case Perks.FastHands:
                m_perksData.m_fastHandsDivider = GameManager.GetInstance().m_gameValues.m_perksData.m_fastHandsDivider;
                break;
            case Perks.Stamina:
                m_perksData.m_extraStamina = GameManager.GetInstance().m_gameValues.m_perksData.m_extraStamina;
                break;
            case Perks.DoubleDamage:
                m_perksData.m_doubleDamageMulti = GameManager.GetInstance().m_gameValues.m_perksData.m_doubleDamageMulti;
                break;
            case Perks.Revive:
                m_perksData.m_revive = GameManager.GetInstance().m_gameValues.m_perksData.m_revive;
                break;
            default:
                break;
        }

        _playerController.m_UIController.m_perksController.AddPerk(perk);
    }

    public void RemovePerk(Perks perk)
    {
        if (!m_activePerks.Contains(perk))
        {
            return;
        }

        m_activePerks.Remove(perk);

        switch (perk)
        {
            case Perks.ExtraHealth:
                _playerController.m_characterHealth.SetExtraHealth(0);
                break;
            case Perks.FastHands:
                m_perksData.m_fastHandsDivider = 1;
                break;
            case Perks.Stamina:
                m_perksData.m_extraStamina = 0;
                break;
            case Perks.DoubleDamage:
                m_perksData.m_doubleDamageMulti = 1;
                break;
            case Perks.Revive:
                m_perksData.m_revive = false;
                break;
            default:
                break;
        }

        _playerController.m_UIController.m_perksController.RemovePerk(perk);
    }

    public void RemoveAllPerks()
    {
        while (m_activePerks.Count != 0)
        {
            RemovePerk(m_activePerks[m_activePerks.Count - 1]);
        }
    }
}


