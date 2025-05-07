using UnityEngine;

/// <summary>
/// Base class for all enemy entities. Handles health, damage reception, and death behavior
/// </summary>
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float m_health = 100f;
    [SerializeField] private Animator m_animatorController;

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
    /// Handles the enemy's death logic
    /// </summary>
    public virtual void Die()
    {
        //TEMP
        GetComponent<CapsuleCollider>().enabled = false;
        m_animatorController.SetTrigger("Die");
    }
}
