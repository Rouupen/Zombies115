using Unity.AppUI.UI;
using Unity.Behavior;
using UnityEngine;

/// <summary>
/// Base class for all enemy entities. Manages enemy behavior and animations
/// </summary>
public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator m_animatorController;
    [SerializeField] private BehaviorGraphAgent m_behaviourAgent;

    private EntityHealth m_enemyHealth;
    private void Awake()
    {
        m_behaviourAgent = GetComponent<BehaviorGraphAgent>();
        m_enemyHealth = GetComponent<EntityHealth>();

        if (m_enemyHealth == null)
        {
            m_enemyHealth = gameObject.AddComponent<EntityHealth>();
            m_enemyHealth.InitializeEntityHealth(100);
            Debug.LogWarning($"{gameObject.name} dosen't have an EntityHealth component attached");
        }

        m_enemyHealth.m_onDeath += Die;
    }

    //TEMP
    public virtual void Attack(GameObject entityGo)
    {
        if (entityGo != null)
        {
            Vector3 origin = transform.position + Vector3.up;
            Vector3 direction = transform.forward;

            Ray ray = new Ray(origin, direction);
            float radius = 0.5f;
            float maxDistance = 2f;

            RaycastHit[] hits = Physics.SphereCastAll(ray, radius, maxDistance);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject != entityGo) continue;

                EntityHealth health = hit.collider.GetComponent<EntityHealth>();
                if (health != null)
                {
                    m_animatorController.SetTrigger("Attack");
                    health.TakeDamage(10);
                    break;
                }
            }
        }
    }


    public virtual void Die()
    {
        //TEMP
        GetComponent<CapsuleCollider>().enabled = false;
        m_behaviourAgent.BlackboardReference.SetVariableValue<bool>("Alive", false);
        m_animatorController.SetTrigger("Die");
    }
}
