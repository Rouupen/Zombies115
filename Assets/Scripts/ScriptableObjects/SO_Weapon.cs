using UnityEngine;

/// <summary>
/// Holds configuration data for a single weapon, including its prefab, stats, and movement behavior
/// </summary>
[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 4)]
public class SO_Weapon : ScriptableObject
{
    public int m_weaponID = -1;
    public GameObject m_weaponPrefab;
    public WeaponStatsData m_weaponStats;
    public WeaponMovementData m_weaponData;
}
