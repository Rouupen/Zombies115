using UnityEngine;

public class ExplosiveWeapon : WeaponBase
{
    public GameObject m_rocketProjectile;
    public GameObject m_rocketReference;


    public override bool Fire()
    {
        if (!base.Fire())
        {
            return false;

        }
        Instantiate(m_rocketProjectile, m_rocketReference.transform.position, GameManager.GetInstance().m_playerController.m_characterLook.transform.rotation);
        m_rocketReference.SetActive(false);

        m_magazineAmmo--;

        if (m_magazineAmmo <= 0 && m_reserveAmmo > 0)
        {
            StartReloadWeapon();
        }
        _playerController.m_UIController.m_ammoController.UpdateAmmoHud(m_magazineAmmo, m_reserveAmmo);
        return true;
    }

    public override void ReloadWeapon(bool updateHud = true)
    {
        base.ReloadWeapon(updateHud);
        m_rocketReference.SetActive(true);

    }
}
