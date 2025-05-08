using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a list of all weapons available in the game
/// </summary>
[CreateAssetMenu(fileName = "WeaponsInGame", menuName = "ScriptableObjects/WeaponsInGame", order = 5)]
public class SO_WeaponsInGame : ScriptableObject
{
    public List<SO_Weapon> m_weaponsInGame;

    public SO_Weapon GetWeaponData(int id)
    {
        for (int i = 0; i < m_weaponsInGame.Count; i++)
        {
            if (m_weaponsInGame[i].m_weaponID == id)
            {
                return m_weaponsInGame[i];
            }
        }

        return null;
    }
}
