using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Behavior;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private float m_health = 100f;

    public delegate void OnDeath();
    public event OnDeath m_onDeath;


    /// <summary>
    /// Initializes the entity with health value
    /// </summary>
    public void InitializeEntityHealth(float health)
    {
        m_health = health;

        if (m_onDeath != null)
        {
            m_onDeath = null;
        }
    }

    /// <summary>
    /// Applies damage to the enemy. Triggers death if health reaches zero or below
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        m_health -= damage;

        if (m_health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Directly modifies the life value. Triggers death if health reaches zero or below
    /// </summary>
    public void SetHealth(float health)
    {
        m_health = health;

        if (m_health <= 0)
        {
            Die();
        }
    }

    public float GetHealth()
    {
        return m_health;
    }

    /// <summary>
    /// Handles the enemy's death logic
    /// </summary>
    public virtual void Die()
    {
        m_onDeath?.Invoke();
    }
}
