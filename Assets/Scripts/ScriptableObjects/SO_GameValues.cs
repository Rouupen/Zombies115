using UnityEngine;

/// <summary>
/// Stores global game configuration values
/// </summary>
[CreateAssetMenu(fileName = "GameValues", menuName = "ScriptableObjects/GameValues", order = 4)]
public class SO_GameValues : ScriptableObject
{
    [Header("Weapon values")]
    [Tooltip("Minimum and maximum fire rate for any weapon in the game")]
    public Vector2 m_minMaxFireRate;
    [Tooltip("Minimum and maximum damage values across all weapons")]
    public Vector2 m_minMaxDamage;
    [Tooltip("Minimum and maximum effective range values for weapons")]
    public Vector2 m_minMaxRange;
    [Tooltip("Minimum and maximum accuracy values")]
    public Vector2 m_minMaxAccuracy;
}
