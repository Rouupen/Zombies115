using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class WeaponStatsData
{

}

public class Weapon : MonoBehaviour
{
    private int _weaponID;
    private WeaponStatsData _weaponStatsData;
    private WeaponMovementData _weaponMovementData;

    private void Start()
    {
    }

    private void Update()
    {

    }

    public void InitializeWeapon(int id, WeaponStatsData weaponStatsData, WeaponMovementData weaponMovementData)
    {
        _weaponID = id;
        _weaponStatsData = weaponStatsData;
        _weaponMovementData = weaponMovementData;
    }

    public int GetWaponID()
    {
        return _weaponID;
    }

    public void SetActiveWeapon()
    {
        if (GameManager.GetInstance().m_playerController.m_weapon != null)
        {
            GameManager.GetInstance().m_playerController.m_weapon.gameObject.SetActive(false);
            GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.Deinitialize();
        }

        gameObject.SetActive(true);
        GameManager.GetInstance().m_playerController.m_weapon = this;
        GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.Initialize(_weaponMovementData);
    }
}
