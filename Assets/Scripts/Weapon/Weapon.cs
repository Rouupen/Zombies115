using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Android.AndroidGame;

[System.Serializable]
public class WeaponStatsData
{
    public bool m_automatic = false;
    public float m_fireRate = 0.1f;
    public float m_minSpamFireRate = 0.1f;
    public List<AudioClip> m_shotSounds;
}

public class Weapon : MonoBehaviour
{
    private int _weaponID;
    private WeaponStatsData _weaponStatsData;
    private WeaponMovementData _weaponMovementData;



    private float _currentFireRate;
    private float _currentMinSpamFireRate;
    private void Start()
    {
    }

    private void Update()
    {
        if (GameManager.GetInstance().m_inputManager.m_fire.IsPressed() && _currentFireRate <= 0 && _currentMinSpamFireRate <= 0 && _weaponStatsData.m_automatic)
        {
            Fire();
        }

        _currentFireRate -= Time.deltaTime;
        _currentMinSpamFireRate -= Time.deltaTime;
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
        GameManager.GetInstance().m_inputManager.m_fire.started += Fire;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if (_currentMinSpamFireRate <= 0)
        {
            _currentMinSpamFireRate = _weaponStatsData.m_minSpamFireRate;
            Fire();
        }
    }

    private void Fire()
    {
        _currentFireRate = _weaponStatsData.m_fireRate;
        GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.Fire();
        if (_weaponStatsData.m_shotSounds.Count != 0)
        {
            AudioSource.PlayClipAtPoint(_weaponStatsData.m_shotSounds[Random.Range(0, _weaponStatsData.m_shotSounds.Count)], transform.position,0.5f);
        }
        GameManager.GetInstance().m_playerController.m_characterLook.ShakeCamera();
    }
}
