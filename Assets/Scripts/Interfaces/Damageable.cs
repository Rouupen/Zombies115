using UnityEngine;

/// <summary>
/// Interface for objects that can receive damage
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Applies damage to the object
    /// </summary>
    void TakeDamage(float damage);
}