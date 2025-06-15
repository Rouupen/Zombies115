using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NormalWeapon : WeaponBase
{
    public override bool Fire()
    {
        if (!base.Fire())
        {
            return false;
        }
        _currentFireRateTime = Mathf.Lerp(GameManager.GetInstance().m_gameValues.m_minMaxFireRate.x, GameManager.GetInstance().m_gameValues.m_minMaxFireRate.y, (20 - _weaponStatsData.m_fireRate) / 20f);
        GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.Fire();
        if (_weaponStatsData.m_shotSounds.Count != 0)
        {
            AudioSource.PlayClipAtPoint(_weaponStatsData.m_shotSounds[UnityEngine.Random.Range(0, _weaponStatsData.m_shotSounds.Count)], transform.position, 0.5f);
        }
        GameManager.GetInstance().m_playerController.m_characterLook.ShakeCamera();
        m_particles.Play();


        for (int i = 0; i < _weaponStatsData.m_numberOfProjectiles; i++)
        {
            Vector3 position = GameManager.GetInstance().m_playerController.m_characterLook.transform.position;
            float angleY = Random.Range(-_weaponStatsData.m_projectilesSpreadAngle, _weaponStatsData.m_projectilesSpreadAngle);
            float angleX = Random.Range(-_weaponStatsData.m_projectilesSpreadAngle, _weaponStatsData.m_projectilesSpreadAngle);
            Quaternion spreadRot = Quaternion.Euler(angleX, angleY, 0);

            Vector3 direction = spreadRot * GameManager.GetInstance().m_playerController.m_characterLook.transform.forward;

            float acuraccy = _currentAcuraccy;
            if (_aiming)
            {
                acuraccy = 0;
            }

            Vector3 rotatedDirection = Quaternion.AngleAxis(UnityEngine.Random.Range(-acuraccy, acuraccy), GameManager.GetInstance().m_playerController.m_characterLook.transform.up) * Quaternion.AngleAxis(UnityEngine.Random.Range(-acuraccy, acuraccy), GameManager.GetInstance().m_playerController.m_characterLook.transform.right) * direction;
            //RaycastHit hitInfo;

            Vector2 minMaxRange = GameManager.GetInstance().m_gameValues.m_minMaxRange;

            float t = _weaponStatsData.m_range / 20.0f;

            float distance = Mathf.Lerp(minMaxRange.x, minMaxRange.y, t);

            Debug.DrawLine(position, position + rotatedDirection * distance, Color.red, 3f);

            int enemyLayer = LayerMask.NameToLayer("Enemy");
            int layerMask = ~(1 << enemyLayer);

            RaycastHit[] hits = Physics.RaycastAll(position, rotatedDirection, distance, layerMask, QueryTriggerInteraction.Ignore);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            List<Enemy> enemysHits = new List<Enemy>();

            foreach (RaycastHit hit in hits)
            {
                Enemy enemy = hit.collider.GetComponentInParent<Enemy>();

                if (enemy != null && enemysHits.Contains(enemy))
                {
                    continue;
                }
                enemysHits.Add(enemy);

                Vector2 minMaxDamage = GameManager.GetInstance().m_gameValues.m_minMaxDamage;
                float tDamage = _weaponStatsData.m_damage / 20.0f;
                float damage = Mathf.Lerp(minMaxDamage.x, minMaxDamage.y, tDamage) * GameManager.GetInstance().m_playerController.m_characterPerks.m_perksData.m_doubleDamageMulti;

                EntityHealth entityHealth = hit.collider.GetComponentInParent<EntityHealth>();
                if (entityHealth != null)
                {
                    entityHealth.TakeDamage(damage / enemysHits.Count);
                }

                if (enemy != null)
                {
                    enemy.StartBloodParticles(hit.point, GameManager.GetInstance().m_playerController.transform.position);
                }
                else
                {
                    break;
                }
            }

            //if (Physics.Raycast(position, rotatedDirection, out RaycastHit hit, distance, layerMask, QueryTriggerInteraction.Ignore))
            //{
            //    Vector2 minMaxDamage = GameManager.GetInstance().m_gameValues.m_minMaxDamage;
            //    float tDamage = _weaponStatsData.m_damage / 20.0f;

            //    float damage = Mathf.Lerp(minMaxDamage.x, minMaxDamage.y, tDamage) * GameManager.GetInstance().m_playerController.m_characterPerks.m_perksData.m_doubleDamageMulti/*/ _weaponStatsData.m_numberOfProjectiles*/;

            //    EntityHealth entityHealth = hit.collider.GetComponentInParent<EntityHealth>();
            //    if (entityHealth != null)
            //    {

            //        entityHealth.TakeDamage(damage);
            //    }
            //    Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            //    if (enemy != null)
            //    {
            //        enemy.StartBloodParticles(hit.point, GameManager.GetInstance().m_playerController.transform.position);
            //    }
            //}
        }




        //if (Physics.Raycast(position, rotatedDirection, out hitInfo, 10))
        //{
        //    Debug.DrawLine(hitInfo.point, hitInfo.point + Vector3.up * 2, Color.red, 3f);
        //}

        m_magazineAmmo--;

        if (m_magazineAmmo <= 0 && m_reserveAmmo > 0)
        {
            StartReloadWeapon();
        }
        _playerController.m_UIController.m_ammoController.UpdateAmmoHud(m_magazineAmmo, m_reserveAmmo);

        return true;

    }
}
