using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PerksSpritesPair
{
    public Perks key;
    public Sprite value;
}


/// <summary>
/// Stores global game configuration values
/// </summary>
[CreateAssetMenu(fileName = "GameValues", menuName = "ScriptableObjects/GameValues", order = 4)]
public class SO_GameValues : ScriptableObject
{
    [Header("Enemys")]
    public List<GameObject> m_enemys;
    public int m_enemyStartHealth;
    public int[] m_enemyHealthAddNextRound;
    public float m_enemyExponentialHealth;
    public Vector2[] m_enemyMinMaxSpeed;
    public Vector2 m_enemyMinMaxSpeedDefault;
    [Header("Weapon values")]
    [Tooltip("Minimum and maximum fire rate for any weapon in the game")]
    public Vector2 m_minMaxFireRate;
    [Tooltip("Minimum and maximum damage values across all weapons")]
    public Vector2 m_minMaxDamage;
    [Tooltip("Minimum and maximum effective range values for weapons")]
    public Vector2 m_minMaxRange;
    [Tooltip("Minimum and maximum accuracy values")]
    public Vector2 m_minMaxAccuracy;

    //TEMP
    [Header("Perks")]
    public PerksData m_perksData;

    [SerializeField]
    private List<PerksSpritesPair> m_spritesPerks;

    
    public Dictionary<Perks, Sprite> m_spritesPerksDictionary
    {
        get
        {
            var dict = new Dictionary<Perks, Sprite>();
            foreach (var pair in m_spritesPerks)
            {
                if (!dict.ContainsKey(pair.key))
                    dict.Add(pair.key, pair.value);
            }
            return dict;
        }
    }
}
