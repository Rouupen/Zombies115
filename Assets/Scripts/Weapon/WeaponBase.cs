using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

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

    public int m_magazineAmmo = 30;
    public int m_totalAmmo = 150;

    [Range(1, 20)]
    public int m_fireRate;
    [Range(1, 20)]
    public int m_damage;
    [Range(1, 20)]
    public int m_range;
    [Range(1, 20)]
    public int m_accuracyIdle;
    [Range(1, 20)]
    public int m_accuracyMoving;

    public int m_numberOfProjectiles = 1;
    public float m_projectilesSpreadAngle = 0;
    public List<AudioClip> m_shotSounds;
}

public class WeaponBase : MonoBehaviour
{
    public ParticleSystem m_particles;

    private int _weaponID;
    public WeaponStatsData _weaponStatsData;
    private WeaponMovementData _weaponMovementData;



    protected float _fireRate;
    protected float _currentFireRateTime;
    protected float _currentMinSpamFireRateTime;
    protected float _currentAcuraccy;

    protected int m_magazineAmmo;
    protected int m_reserveAmmo;

    private Animator _animatorController;
    private Coroutine _reloadAnimation;
    private Coroutine _changeWeaponAnimation;

    //temp
    private bool _canShoot;
    protected bool _aiming;
    protected PlayerController _playerController;
    private void Update()
    {
        if (GameManager.GetInstance().m_inputManager.m_fire.IsPressed() && _currentFireRateTime <= 0 && _currentMinSpamFireRateTime <= 0 && _weaponStatsData.m_shootingMode == ShootingMode.Automatic)
        {
            Fire();
        }

        _currentFireRateTime -= Time.deltaTime;
        _currentMinSpamFireRateTime -= Time.deltaTime;

        EnemyDetection();
    }


    private void EnemyDetection()
    {
        Vector3 position = _playerController.m_characterLook.transform.position;
        Vector3 direction = _playerController.m_characterLook.transform.forward;

        Vector2 minMaxRange = GameManager.GetInstance().m_gameValues.m_minMaxRange;

        float t = _weaponStatsData.m_range / 20.0f;

        float distance = Mathf.Lerp(minMaxRange.x, minMaxRange.y, t);

        int enemyLayer = LayerMask.NameToLayer("EnemyRagdoll");
        int layerMask = 1 << enemyLayer;

        if (Physics.Raycast(position, direction, out RaycastHit hit, distance, layerMask, QueryTriggerInteraction.Ignore) && hit.collider.GetComponentInParent<EntityHealth>().GetCurrentHealth() > 0)
        {
            _playerController.m_UIController.m_crosshairController.SetColor(Color.red);
        }
        else
        {
            _playerController.m_UIController.m_crosshairController.SetColor(Color.white);
        }
    }

    public void InitializeWeapon(int id, WeaponStatsData weaponStatsData, WeaponMovementData weaponMovementData, PlayerController player)
    {
        _weaponID = id;
        _weaponStatsData = weaponStatsData;
        _weaponMovementData = weaponMovementData;

        m_reserveAmmo = _weaponStatsData.m_totalAmmo;
        _playerController = player;
        //Temp 
        _animatorController = GetComponent<Animator>();

        ReloadWeapon();

    }

    public int GetWeaponID()
    {
        return _weaponID;
    }

    public void SetActiveWeapon()
    {

        if (_changeWeaponAnimation != null)
        {
            StopCoroutine(_changeWeaponAnimation);
        }
        _changeWeaponAnimation = GameManager.GetInstance().StartCoroutine(ChangeWeaponAnimation());
        
    }
    private void OnDestroy()
    {
        GameManager.GetInstance().m_inputManager.m_fire.started -= Fire;
        GameManager.GetInstance().m_inputManager.m_reload.started -= StartReloadWeapon;
    }
    public void HideWeapon()
    {
        _canShoot = false;
        _animatorController.SetTrigger("ChangeWeapon");

    }
    public void ShowWeapon()
    {
        _canShoot = true;
        _animatorController.SetTrigger("SelectWeapon");
    }
    private IEnumerator ChangeWeaponAnimation()
    {

        if (GameManager.GetInstance().m_playerController.GetCurrentWeapon() != null)
        {
            if (GameManager.GetInstance().m_playerController.GetCurrentWeapon() == this)
            {
                yield break;
            }
        }

        if (GameManager.GetInstance().m_playerController.GetCurrentWeapon() == null)
        {
            gameObject.SetActive(true);
            GameManager.GetInstance().m_playerController.SetCurrentWeapon(this);
            GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.Initialize(_weaponMovementData);

            //temp
            //GameManager.GetInstance().m_crosshairController.RotateCrosshair();
            GameManager.GetInstance().m_inputManager.m_fire.started += Fire;
            GameManager.GetInstance().m_inputManager.m_reload.started += StartReloadWeapon;
            _canShoot = true;

            //CHANGE BUG
            SetIdleValues();
            yield break;

        }

        _canShoot = false;
        float timeChange = 0;
        float timeSelect = 0;

        float speed = GameManager.GetInstance().m_playerController.m_characterPerks.m_perksData.m_fastHandsDivider;

        _animatorController.speed = speed;

        RuntimeAnimatorController rac = GameManager.GetInstance().m_playerController.GetCurrentWeapon().GetAnimator().runtimeAnimatorController;
        GameManager.GetInstance().m_playerController.GetCurrentWeapon().GetAnimator().SetTrigger("ChangeWeapon");
        foreach (AnimationClip clip in rac.animationClips)
        {
            if (clip.name.Contains("ChangeWeapon"))
            {
                timeChange += clip.length;
                break;
            }
        }

        RuntimeAnimatorController thisRac =_animatorController.runtimeAnimatorController;

        foreach (AnimationClip clip in thisRac.animationClips)
        {
            if (clip.name.Contains("SelectWeapon"))
            {
                timeSelect += clip.length;
                break;
            }
        }

        _playerController.m_UIController.m_crosshairController.RotateCrosshair(timeChange / speed + timeSelect / speed);

        if (GameManager.GetInstance().m_playerController.GetCurrentWeapon() != null)
        {
            GameManager.GetInstance().m_inputManager.m_fire.started -= GameManager.GetInstance().m_playerController.GetCurrentWeapon().Fire;
            GameManager.GetInstance().m_inputManager.m_reload.started -= GameManager.GetInstance().m_playerController.GetCurrentWeapon().StartReloadWeapon;
            GameManager.GetInstance().m_playerController.GetCurrentWeapon()._canShoot = false;
            GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.EndAim();
            GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.CanAim(false);
        }

        float currentTime = 0;
        while (currentTime <= timeChange/ speed)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        if (GameManager.GetInstance().m_playerController.GetCurrentWeapon() != null)
        {
            GameManager.GetInstance().m_playerController.GetCurrentWeapon().gameObject.SetActive(false);
            GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.Deinitialize();
        }



        _animatorController.SetTrigger("SelectWeapon");

        gameObject.SetActive(true);
        GameManager.GetInstance().m_playerController.SetCurrentWeapon(this);
        GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.Initialize(_weaponMovementData);
        GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.CanAim(false);

        //temp
        GameManager.GetInstance().m_inputManager.m_fire.started += Fire;
        GameManager.GetInstance().m_inputManager.m_reload.started += StartReloadWeapon;


        //CHANGE BUG
        SetIdleValues();

        _playerController.m_UIController.m_ammoController.UpdateAmmoHud(m_magazineAmmo, m_reserveAmmo);


        if (_reloadAnimation != null)
        {
            StopCoroutine(_reloadAnimation);
            _reloadAnimation = null;
        }

        currentTime = 0;
        while (currentTime <= timeSelect / speed)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        _animatorController.speed = 1;

        _canShoot = true;
        GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.CanAim(true);

        if (GameManager.GetInstance().m_inputManager.m_fire.IsPressed())
        {
            Fire();
        }
    }


    private void Fire(InputAction.CallbackContext context)
    {
        if (_currentMinSpamFireRateTime <= 0)
        {
            _currentMinSpamFireRateTime = Mathf.Lerp(GameManager.GetInstance().m_gameValues.m_minMaxFireRate.x, GameManager.GetInstance().m_gameValues.m_minMaxFireRate.y, (20 - _weaponStatsData.m_fireRate) / 20f);
            Fire();
        }
    }

    public virtual bool Fire()
    {
        if (!_canShoot)
        {
            return false;
        }

        if (m_magazineAmmo <= 0)
        {
            if (m_reserveAmmo > 0)
            {
                StartReloadWeapon();
            }
            return false;
        }

        return true;

        //_currentFireRateTime = Mathf.Lerp(GameManager.GetInstance().m_gameValues.m_minMaxFireRate.x, GameManager.GetInstance().m_gameValues.m_minMaxFireRate.y, (20 - _weaponStatsData.m_fireRate) / 20f);
        //GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.Fire();
        //if (_weaponStatsData.m_shotSounds.Count != 0)
        //{
        //    AudioSource.PlayClipAtPoint(_weaponStatsData.m_shotSounds[UnityEngine.Random.Range(0, _weaponStatsData.m_shotSounds.Count)], transform.position, 0.5f);
        //}
        //GameManager.GetInstance().m_playerController.m_characterLook.ShakeCamera();
        //m_particles.Play();


        //Vector3 position = GameManager.GetInstance().m_playerController.m_characterLook.transform.position;
        //Vector3 direction = GameManager.GetInstance().m_playerController.m_characterLook.transform.forward;
        //Vector3 rotatedDirection = Quaternion.AngleAxis(-20, GameManager.GetInstance().m_playerController.m_characterLook.transform.right) * direction;
        //RaycastHit hitInfo;

        //Vector2 minMaxRange = GameManager.GetInstance().m_gameValues.m_minMaxRange;

        //float t = _weaponStatsData.m_range / 20.0f;

        //float distance = Mathf.Lerp(minMaxRange.x, minMaxRange.y, t);

        //Debug.DrawLine(position, position + direction * distance, Color.red, 3f);

        //int enemyLayer = LayerMask.NameToLayer("Enemy");
        //int layerMask = ~(1 << enemyLayer);

        //if (Physics.Raycast(position, direction, out RaycastHit hit, distance, layerMask, QueryTriggerInteraction.Ignore))
        //{
        //    Vector2 minMaxDamage = GameManager.GetInstance().m_gameValues.m_minMaxDamage;
        //    float tDamage = _weaponStatsData.m_damage / 20.0f;

        //    float damage = Mathf.Lerp(minMaxDamage.x, minMaxDamage.y, tDamage);

        //    EntityHealth entityHealth = hit.collider.GetComponentInParent<EntityHealth>();
        //    if (entityHealth != null)
        //    {
        //        entityHealth.TakeDamage(damage);
        //    }
        //}


        ////if (Physics.Raycast(position, rotatedDirection, out hitInfo, 10))
        ////{
        ////    Debug.DrawLine(hitInfo.point, hitInfo.point + Vector3.up * 2, Color.red, 3f);
        ////}

        //m_magazineAmmo--;

        //if (m_magazineAmmo <= 0 && m_reserveAmmo > 0)
        //{
        //    StartReloadWeapon();
        //}
        //GameManager.GetInstance().m_ammoController.UpdateAmmoHud(m_magazineAmmo, m_reserveAmmo);
    }




    public void SetIdleValues()
    {
        _playerController.m_UIController.m_crosshairController.SetCurrentAccuracy(_weaponStatsData.m_accuracyIdle);
        _currentAcuraccy = 20 - _weaponStatsData.m_accuracyIdle;
    }

    public void SetMovingValues()
    {
        _playerController.m_UIController.m_crosshairController.SetCurrentAccuracy(_weaponStatsData.m_accuracyMoving);
        _currentAcuraccy = 20 - _weaponStatsData.m_accuracyMoving;
    }



    public void StartReloadWeapon(InputAction.CallbackContext context)
    {
        StartReloadWeapon();
    }
    public void StartReloadWeapon()
    {
        if (m_magazineAmmo >= _weaponStatsData.m_magazineAmmo || m_reserveAmmo <= 0)
        {
            return;
        }


        if (_reloadAnimation != null)
        {
            return;
            //StopCoroutine(_reloadAnimation);
        }
        _reloadAnimation = StartCoroutine(ReloadWeaponAnimation());
    }

    IEnumerator ReloadWeaponAnimation()
    {
        GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.EndAim();
        GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.CanAim(false);
        _canShoot = false;
        _animatorController.SetTrigger("Reload");

        float speed = GameManager.GetInstance().m_playerController.m_characterPerks.m_perksData.m_fastHandsDivider;

        _animatorController.speed = speed;

        RuntimeAnimatorController rac = _animatorController.runtimeAnimatorController;
        float time = 0;

        foreach (AnimationClip clip in rac.animationClips)
        {
            if (clip.name.Contains("Reloading"))
            {
                time = clip.length;
                break;
            }
        }

        float currentTime = 0;
        while (currentTime <= time / speed)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        _reloadAnimation = null;
        ReloadWeapon();
        _canShoot = true;
        GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.CanAim(true);
        _animatorController.speed = 1;
        if (GameManager.GetInstance().m_inputManager.m_aim.IsPressed())
        {
            GameManager.GetInstance().m_playerController.m_weaponSocketMovementController.StartAim();
        }
    }

    public virtual void ReloadWeapon()
    {
        if (m_reserveAmmo > 0)
        {
            int ammoReloded = Mathf.Clamp(_weaponStatsData.m_magazineAmmo - m_magazineAmmo, 0, m_reserveAmmo);
            m_reserveAmmo -= ammoReloded;
            m_magazineAmmo += ammoReloded;
        }

        _playerController.m_UIController.m_ammoController.UpdateAmmoHud(m_magazineAmmo, m_reserveAmmo);
    }



    public void Aiming(bool aiming)
    {
        _animatorController.SetBool("Idle", !aiming);
        _aiming = aiming;
    }

    public Animator GetAnimator()
    {
        return _animatorController;
    }


    //private void OnDrawGizmos()
    //{
    //    if (!GameManager.GetInstance())
    //    {
    //        return;
    //    }
    //    Vector3 position = GameManager.GetInstance().m_playerController.m_characterLook.transform.position;
    //    Vector3 direction = GameManager.GetInstance().m_playerController.m_characterLook.transform.forward;
    //    Vector3 rotatedDirection = Quaternion.AngleAxis(UnityEngine.Random.Range(-20,20.0f), GameManager.GetInstance().m_playerController.m_characterLook.transform.up) * Quaternion.AngleAxis(UnityEngine.Random.Range(-20, 20.0f), GameManager.GetInstance().m_playerController.m_characterLook.transform.right) * direction;

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(position + rotatedDirection, 0.01f);

    //    //Max 425
    //    //min 9.5
    //}
}

















