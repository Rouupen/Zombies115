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
    private GameObject m_target;
    private Rigidbody[] m_rigidbodies;
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

        m_rigidbodies = transform.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in m_rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    }

    //TEMP
    public virtual void Attack()
    {
        if (m_target != null)
        {
            Vector3 origin = transform.position + Vector3.up;
            Vector3 direction = transform.forward;

            Ray ray = new Ray(origin, direction);
            float radius = 0.5f;
            float maxDistance = 2f;

            RaycastHit[] hits = Physics.SphereCastAll(ray, radius, maxDistance);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject != m_target) continue;

                EntityHealth health = hit.collider.GetComponent<EntityHealth>();
                if (health != null)
                {
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
        m_animatorController.SetFloat("DeathAnim",Random.Range(0,4));
        m_animatorController.SetTrigger("Die");
    }

    public void DisableAnimator()
    {
        foreach (Rigidbody rigidbody in m_rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        m_animatorController.enabled = false;
    }

    public void PlayTriggerAnimation(string animation)
    {
        m_animatorController.SetTrigger(animation);
    }
    public void UpdateTarget(GameObject target)
    {
        m_target = target;
    }
}
