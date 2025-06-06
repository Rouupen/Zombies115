using System.Collections;
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

    [HideInInspector] public EntityHealth m_enemyHealth;
    private GameObject m_target;
    private Rigidbody[] m_rigidbodies;
    private Coroutine m_deadCoroutine;
    private bool m_instaKill;
    public bool m_isDead = true;
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
        m_isDead = true;
        m_enemyHealth.m_onDeath += Die;

        m_enemyHealth.m_onDamageTaked += DamageTaked;

        m_rigidbodies = transform.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in m_rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    }

    public void InitializeEnemy(SpawnType spawnType, float health, float speed)
    {

        switch (spawnType)
        {
            case SpawnType.Instant:
                m_behaviourAgent.BlackboardReference.SetVariableValue<bool>("InstantSpawn", true);
                m_animatorController.SetBool("InstantSpawn", true);
                break;
            case SpawnType.Ground:
                m_behaviourAgent.BlackboardReference.SetVariableValue<bool>("InstantSpawn", false);
                m_animatorController.SetBool("InstantSpawn", false);

                break;
            default:
                break;
        }
        m_behaviourAgent.BlackboardReference.SetVariableValue<bool>("Alive", true);

        m_enemyHealth.InitializeEntityHealth(health);
        m_behaviourAgent.BlackboardReference.SetVariableValue<float>("Speed", speed);
        m_animatorController.SetFloat("Speed", speed);
        m_animatorController.enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        m_isDead = false;
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
                    health.TakeDamage(50);
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
        m_animatorController.SetFloat("DeathAnim", Random.Range(0, 4));
        m_animatorController.SetTrigger("Die");
        GameManager.GetInstance().m_playerController.m_UIController.m_pointsController.AddPoints(100);
        GameManager.GetInstance().m_playerController.m_UIController.m_scoreController.m_kills++;

        GameManager.GetInstance().m_powerUpManager.TryToInstatiateRandomNewPowerUp(transform.position);
        m_isDead = true;
        DieDespawn();
    }

    public virtual void DamageTaked()
    {
        GameManager.GetInstance().m_playerController.m_UIController.m_pointsController.AddPoints(10);

        if (m_instaKill)
        {
            m_enemyHealth.Die();
        }
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

    public void SetTarget(GameObject target)
    {
        m_behaviourAgent.BlackboardReference.SetVariableValue<Transform>("Target", target.transform);
    }

    void DieDespawn()
    {
        if (m_deadCoroutine != null)
        {
            StopCoroutine(m_deadCoroutine);
        }

        m_deadCoroutine = StartCoroutine(DieDespawnAnimation());
    }

    IEnumerator DieDespawnAnimation()
    {
        yield return new WaitForSeconds(15);
        gameObject.SetActive(false);
        m_deadCoroutine = null;
    }

    public void SetInstaKill(bool instaKill)
    {
        m_instaKill = instaKill;
    }
}
