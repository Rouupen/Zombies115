using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Behavior;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private float m_health = 100f;

    public delegate void OnDeath();
    public event OnDeath m_onDeath;
    
    public delegate void OnDamageTaked();
    public event OnDamageTaked m_onDamageTaked;

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = m_health;
    }

    /// <summary>
    /// Initializes the entity with health value
    /// </summary>
    public void InitializeEntityHealth(float health)
    {
        m_health = health;
        _currentHealth = m_health;

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
        if (_currentHealth <= 0)
        {
            return;
        }

        _currentHealth -= damage;

        m_onDamageTaked?.Invoke();
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Directly modifies the life value. Triggers death if health reaches zero or below
    /// </summary>
    public void SetHealth(float health)
    {
        _currentHealth = health;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

    public float GetTotalHealth()
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
