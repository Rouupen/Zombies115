using UnityEngine;


//TEMP
public class RocketProjectile : MonoBehaviour
{
    public ParticleSystem m_particles;
    private void Awake()
    {
        Destroy(gameObject,10.0f);
    }


    void Update()
    {
        transform.position += transform.right * 20 * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null || other.isTrigger)
        {
            return;
        }
        int enemyLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << enemyLayer);
        Collider[] obj = Physics.OverlapSphere(transform.position, 5, layerMask);
        foreach (Collider c in obj)
        {
            //Bug -  For now works
            if (c.TryGetComponent<PlayerController>(out PlayerController playerController))
            {
                return;
            }
            else if (c.TryGetComponent<EntityHealth>(out EntityHealth entityHealth))
            {
                entityHealth.TakeDamage(200);
            }
        }

        ParticleSystem particles = Instantiate(m_particles, transform.position, transform.rotation);
        particles.Play();
        Destroy(particles, 5);

        Destroy(gameObject);
    }
}
