using System.Collections;
using UnityEngine;

public class CharacterHealth : EntityHealth
{
    private Coroutine _recoveringHealth;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        float blend = 1 - GetCurrentHealth() / GetTotalHealth();
        GameManager.GetInstance().m_damageController.SetDamageBlend(blend);


        if (_recoveringHealth != null)
        {
            StopCoroutine(_recoveringHealth);
        }

        if (GetCurrentHealth() <= 0)
        {
            return;
        }
        _recoveringHealth = StartCoroutine(RecoveringHealth());

    }

    private IEnumerator RecoveringHealth()
    {
        //TEMP
        float currentTime = 0;
        float timeStartRecovering = 3;
        float timeToRecoverTotalHealth = 1;

        while (currentTime <= timeStartRecovering)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        currentTime = 0;

        float startHealth = GetCurrentHealth();

        while (currentTime <= timeToRecoverTotalHealth)
        {
            float newHealth = Mathf.Lerp(startHealth, GetTotalHealth(), currentTime / timeToRecoverTotalHealth);
            SetHealth(newHealth);

            float blend = 1 - GetCurrentHealth() / GetTotalHealth();
            GameManager.GetInstance().m_damageController.SetDamageBlend(blend);
            currentTime += Time.deltaTime;
            yield return null;
        }
        GameManager.GetInstance().m_damageController.SetDamageBlend(0);
        _recoveringHealth = null;
    }
}
