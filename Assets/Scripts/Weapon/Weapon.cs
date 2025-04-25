using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ShootingMode
{
    SemiAutomatic,
    Automatic,
    Burst
}

[System.Serializable]
public class WeaponStatsData
{
    public ShootingMode m_shootingMode;

    [Range(1, 20)]
    public int m_fireRate;
    [Range(1, 20)]
    public int m_damage;
    [Range(1, 20)]
    public int m_range;
    [Range(1, 20)]
    public int m_accuracy;


    public List<AudioClip> m_shotSounds;
}

public class Weapon : MonoBehaviour
{
    public ParticleSystem m_particles;

    private int _weaponID;
    public WeaponStatsData _weaponStatsData;
    private WeaponMovementData _weaponMovementData;



    private float _fireRate;
    private float _currentFireRateTime;
    private float _currentMinSpamFireRateTime;
    private void Start()
    {
    }

    private void Update()
    {
        if (GameManager.GetInstance().m_inputManager.m_fire.IsPressed() && _currentFireRateTime <= 0 && _currentMinSpamFireRateTime <= 0 && _weaponStatsData.m_shootingMode == ShootingMode.Automatic)
        {
            Fire();
        }

        _currentFireRateTime -= Time.deltaTime;
        _currentMinSpamFireRateTime -= Time.deltaTime;
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

        //temp
        GameManager.GetInstance().m_crosshairController.RotateCrosshair();
        GameManager.GetInstance().m_inputManager.m_fire.started += Fire;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if (_currentMinSpamFireRateTime <= 0)
        {
            _currentMinSpamFireRateTime = Mathf.Lerp(GameManager.GetInstance().m_gameValues.m_minMaxFireRate.x, GameManager.GetInstance().m_gameValues.m_minMaxFireRate.y, (20 - _weaponStatsData.m_fireRate) / 20f);
            Fire();
        }
    }

    private void Fire()
    {
        _currentFireRateTime = Mathf.Lerp(GameManager.GetInstance().m_gameValues.m_minMaxFireRate.x, GameManager.GetInstance().m_gameValues.m_minMaxFireRate.y, (20 - _weaponStatsData.m_fireRate) / 20f);
        GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.Fire();
        if (_weaponStatsData.m_shotSounds.Count != 0)
        {
            AudioSource.PlayClipAtPoint(_weaponStatsData.m_shotSounds[UnityEngine.Random.Range(0, _weaponStatsData.m_shotSounds.Count)], transform.position, 0.5f);
        }
        GameManager.GetInstance().m_playerController.m_characterLook.ShakeCamera();
        m_particles.Play();


        Vector3 position = GameManager.GetInstance().m_playerController.m_characterLook.transform.position;
        Vector3 direction = GameManager.GetInstance().m_playerController.m_characterLook.transform.forward;
        Vector3 rotatedDirection = Quaternion.AngleAxis(-20, GameManager.GetInstance().m_playerController.m_characterLook.transform.right) * direction;
        RaycastHit hitInfo;
        if (Physics.Raycast(position, rotatedDirection, out hitInfo, 10))
        {
            Debug.DrawLine(hitInfo.point, hitInfo.point + Vector3.up * 2, Color.red, 3f);
        }


    }

    private void OnDrawGizmos()
    {
        Vector3 position = GameManager.GetInstance().m_playerController.m_characterLook.transform.position;
        Vector3 direction = GameManager.GetInstance().m_playerController.m_characterLook.transform.forward;
        Vector3 rotatedDirection = Quaternion.AngleAxis(-20, GameManager.GetInstance().m_playerController.m_characterLook.transform.right) * direction;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(position + rotatedDirection, 0.01f);

        //Max 425
        //min 9.5
    }
}
