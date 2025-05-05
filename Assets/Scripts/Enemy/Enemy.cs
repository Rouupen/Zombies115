using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public Animator m_animationController;
    public float health = 100f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        m_animationController.SetTrigger("Die");
    }
}
