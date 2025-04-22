using UnityEngine;

[CreateAssetMenu(fileName = "GameValues", menuName = "ScriptableObjects/GameValues", order = 4)]
public class SO_GameValues : ScriptableObject
{
    [Header("Weapon values")]
    public Vector2 m_minMaxFireRate;
    public Vector2 m_minMaxDamage;
    public Vector2 m_minMaxRange;
    public Vector2 m_minMaxAccuracy;
}
