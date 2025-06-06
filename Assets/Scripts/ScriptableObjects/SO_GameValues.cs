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

[Serializable]
public class PowerUpModelPair
{
    public PowerUps m_type;
    public GameObject m_model;
    public Sprite sprite;

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

    [Header("PowerUps")]
    public GameObject m_powerUpObject;

    [SerializeField]
    private List<PowerUpModelPair> m_powerUpModel;

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

    public Dictionary<PowerUps, GameObject> m_powerUpModelDictionary
    {
        get
        {
            var dict = new Dictionary<PowerUps, GameObject>();
            foreach (var pair in m_powerUpModel)
            {
                if (!dict.ContainsKey(pair.m_type))
                    dict.Add(pair.m_type, pair.m_model);
            }
            return dict;
        }
    }

    public Dictionary<PowerUps, Sprite> m_powerUpSpriteDictionary
    {
        get
        {
            var dict = new Dictionary<PowerUps, Sprite>();
            foreach (var pair in m_powerUpModel)
            {
                if (!dict.ContainsKey(pair.m_type))
                    dict.Add(pair.m_type, pair.sprite);
            }
            return dict;
        }
    }
}
