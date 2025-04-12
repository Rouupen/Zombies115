using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

//TESTING - Spaghetti code for now :)
public class Weapon : MonoBehaviour
{
    [Header("---FAST TESTING---")]
    [Header("Look Inertia")]
    public float m_maxAngleInertia = 10.0f;
    public float m_maxTranslationInertia = 10.0f;
    public float m_inertiaSpeed = 5.0f;
    public float m_returnToOriginalPositionInertiaSpeed = 7.0f;

    [Header("Recoil Inertia")]
    public float m_recoilUpPositionSpeed = 0.05f;
    public float m_recoilRightRotationSpeed = -20f;
    public float m_recoilTime = 0.05f;
    public float m_recoilRecoverySpeedTime = 0.15f;
    public Vector2 m_randomRotationInertia = new Vector2(1, 5);
    public AnimationCurve m_recoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Aim")]
    public Vector3 m_aimPosition;
    public Quaternion m_aimRotation;
    public float m_startAimTime = 0.2f;
    public float m_endAimTime = 0.2f;
    public float m_inertiaReductionWhileAiming = 0.1f;
    [Header("Idle")]
    public Vector3 m_localPositionWeaponIdle;

    [Header("Walking - Start/End")]
    public Vector3 m_localPositionWeaponWalking;
    public AnimationCurve m_localPositionWalkingTranslationCurve;
    public float m_localPositionWeaponWalkingTime = 0.15f;

    [Header("Walking - Direction translation")]
    public AnimationCurve m_translationCurve;
    public float m_translationHorizontalDistance = 0.5f;
    public float m_translationForwardDistance = 0.1f;
    public float m_translationTime = 0.25f;

    [Header("Walking - Loop")]
    public AnimationCurve m_bounceVerticalCurve;
    public AnimationCurve m_bounceHorizontalCurve;
    public AnimationCurve m_rotationLoopForwardCurve;
    public float m_bounceVerticalTime = 0.5f;
    public float m_bounceHorizontalTime = 1f;
    public float m_rotationLoopForwardTime = 1f;
    public float m_bounceHorizontalAmplitude = 0.03f;
    public float m_bounceVerticalAmplitude = 0.02f;
    public float m_rotationForwardAmplitude = 2f;

    //Walking private values
    private float _currentVerticalBounceTime = 0f;
    private float _currentHorizontalBounceTime = 0f;
    private float _currentForwardRotationTime = 0f;
    private Coroutine _startEndWalking;
    private bool _isWalking;

    [Header("Damping")]
    public AnimationCurve m_dampingCurve;
    public float m_dampingTime = 0.35f;
    public float m_dampingAmplitude = 0.1f;


    private Transform _socket;
    private Quaternion _socketOriginalRotation;
    private Vector3 _socketOriginalPosition;
    private Vector3 _currentSocketPosition;
    private Quaternion _lastRotation;
    private Coroutine _coroutineFire;
    private Coroutine _coroutineAim;
    private Coroutine _coroutineDamping;
    private bool _isRecoiling = false;
    private bool _isAiming = false;



    //Temp
    bool _wasGrounded = true;
    private void Start()
    {
        _socket = transform.parent;
        _socketOriginalRotation = _socket.localRotation;
        _socketOriginalPosition = _socket.localPosition;
        _currentSocketPosition = _socketOriginalPosition;
        _lastRotation = _socketOriginalRotation;

        GameManager.GetInstance().m_inputManager.m_fire.started += Fire;
        GameManager.GetInstance().m_inputManager.m_aim.started += StartAim;
        GameManager.GetInstance().m_inputManager.m_aim.canceled += EndAim;
    }

    private void LateUpdate()
    {
        //Ground damping
        if (!_wasGrounded && GameManager.GetInstance().m_playerController.m_characterController.isGrounded)
        {
            StartDamping();
        }
        _wasGrounded = GameManager.GetInstance().m_playerController.m_characterController.isGrounded;

        if (_isRecoiling /*|| _isAiming*/)
        {
            return;
        }
        //float recoilInertiaReduction = _isAiming ? m_inertiaReductionWhileAiming : 1;

        //Vector2 dir = GameManager.GetInstance().m_inputManager.m_look.ReadValue<Vector2>();
        //dir.y += GameManager.GetInstance().m_playerController.m_characterController.velocity.normalized.y;
        //dir = Vector2.ClampMagnitude(dir, 1f);
        //if (dir == Vector2.zero)
        //{
        //    float angleDiff = Quaternion.Angle(_socket.localRotation, _socketOriginalRotation);
        //    float returnSpeed = (angleDiff / m_maxAngleInertia) * m_returnToOriginalPositionInertiaSpeed;
        //    _socket.localRotation = Quaternion.Slerp(_socket.localRotation, _socketOriginalRotation, Time.deltaTime * returnSpeed);
        //    _socket.localPosition = Vector3.Slerp(_socket.localPosition, _socketOriginalPosition, Time.deltaTime * returnSpeed);
        //}
        //else
        //{
        //    Quaternion targetRot = Quaternion.Euler(-dir.y * m_maxAngleInertia / 3 * recoilInertiaReduction, dir.x * m_maxAngleInertia * recoilInertiaReduction, 0f);
        //    Vector3 targetPos = new Vector3(-dir.x * m_maxTranslationInertia * recoilInertiaReduction, 0, 0f);
        //    _socket.localRotation = Quaternion.Slerp(_socket.localRotation, _socketOriginalRotation * targetRot, Time.deltaTime * m_inertiaSpeed);
        //    _socket.localPosition = Vector3.Slerp(_socket.localPosition, _socketOriginalPosition + targetPos, Time.deltaTime * m_inertiaSpeed);
        //}

        WalkingLoop();
    }

    private void StartEndWalking(bool starting)
    {
        if (_startEndWalking != null)
        {
            StopCoroutine(_startEndWalking);
        }
        _isWalking = starting;
        _startEndWalking = StartCoroutine(StartEndWalkAnimation(starting));
    }

    private IEnumerator StartEndWalkAnimation(bool starting)
    {
        float currentTime = 0;
        Vector3 startPosition = _socket.localPosition;
        Vector3 endPosition = starting ? m_localPositionWeaponWalking : m_localPositionWeaponIdle;
        while (currentTime <= m_localPositionWeaponWalkingTime)
        {
            Vector3 currentPosition = Vector3.Slerp(_currentSocketPosition, endPosition, m_localPositionWalkingTranslationCurve.Evaluate((currentTime / m_localPositionWeaponWalkingTime) % 1));
            _currentSocketPosition = currentPosition;
            _socket.localPosition = currentPosition;
            currentTime += Time.deltaTime;
            yield return null;
        }
        _currentSocketPosition = endPosition;
        _socket.localPosition = endPosition;
        _startEndWalking = null;
    }
    private void WalkingLoop()
    {
        //WALKING
        if (!_isAiming && GameManager.GetInstance().m_playerController.m_characterController.isGrounded)
        {
            Vector3 characterVelocity = GameManager.GetInstance().m_playerController.m_characterController.velocity;
            characterVelocity.y = 0f; //Ingore the Y-axies

            if (characterVelocity.magnitude > 0.1f)
            {
                //Temp - Need state machine
                if (!_isWalking)
                {
                    StartEndWalking(true);
                }

                _currentVerticalBounceTime += Time.deltaTime / m_bounceVerticalTime;
                _currentHorizontalBounceTime += Time.deltaTime / m_bounceHorizontalTime;
                _currentForwardRotationTime += Time.deltaTime / m_rotationLoopForwardTime;

                Vector3 newPosition = new Vector3(
                    m_bounceHorizontalCurve.Evaluate(_currentHorizontalBounceTime % 1) * m_bounceHorizontalAmplitude,
                    m_bounceVerticalCurve.Evaluate(_currentVerticalBounceTime % 1) * m_bounceVerticalAmplitude,
                    0.0f
                    );

                Quaternion newRotation = Quaternion.Euler(
                    _socketOriginalRotation.x,
                    _socketOriginalRotation.y,
                    _socketOriginalRotation.z + m_rotationLoopForwardCurve.Evaluate(_currentForwardRotationTime % 1) * m_rotationForwardAmplitude
                    );

                _socket.localPosition = _currentSocketPosition + newPosition;
                _socket.localRotation = newRotation;
            }
            else
            {
                if (_isWalking)
                {
                    StartEndWalking(false);
                }
                _currentVerticalBounceTime = 0f;
                _currentHorizontalBounceTime = 0f;
                _currentForwardRotationTime = 0f;
                //_socket.localPosition = Vector3.Slerp(_socket.localPosition, _socketOriginalPosition, Time.deltaTime * m_inertiaSpeed);
            }
        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if (_coroutineFire != null)
        {
            StopCoroutine(_coroutineFire);
        }
        _coroutineFire = StartCoroutine(FireAnimation());
    }

    IEnumerator FireAnimation()
    {
        _isRecoiling = true;
        float _currentRecoilSpeedTime = 0;
        float _currentRecoilRecoverySpeedTime = 0;
        Vector3 currentPosition = _currentSocketPosition;
        Quaternion currentRotation = _socket.localRotation;
        currentRotation.y = _lastRotation.y;
        float recoilInertiaReduction = _isAiming ? m_inertiaReductionWhileAiming : 1;
        Vector2 randomRotation = new Vector2(UnityEngine.Random.Range(-1, 1.0f), UnityEngine.Random.Range(-1, 1.0f)) * UnityEngine.Random.Range(m_randomRotationInertia.x, m_randomRotationInertia.y);
        while (_currentRecoilSpeedTime <= m_recoilTime)
        {
            float t = _currentRecoilSpeedTime / m_recoilTime;
            float curveT = m_recoilCurve.Evaluate(t);
            _socket.localPosition = Vector3.Slerp(currentPosition, currentPosition + Vector3.up * m_recoilUpPositionSpeed * recoilInertiaReduction, curveT);
            Quaternion recoilRot = Quaternion.Euler(m_recoilRightRotationSpeed * recoilInertiaReduction, randomRotation.y, randomRotation.x);
            _socket.localRotation = Quaternion.Slerp(currentRotation, currentRotation * recoilRot, curveT);
            _currentRecoilSpeedTime += Time.deltaTime;
            yield return null;
        }

        while (_currentRecoilRecoverySpeedTime <= m_recoilRecoverySpeedTime)
        {
            _socket.localPosition = Vector3.Slerp(currentPosition + Vector3.up * m_recoilUpPositionSpeed * recoilInertiaReduction, currentPosition, _currentRecoilRecoverySpeedTime / m_recoilRecoverySpeedTime);
            _socket.localRotation = Quaternion.Slerp(_socket.localRotation, currentRotation, _currentRecoilRecoverySpeedTime / m_recoilRecoverySpeedTime);
            _currentRecoilRecoverySpeedTime += Time.deltaTime;
            yield return null;

        }
        _isRecoiling = false;
        _coroutineFire = null;
    }

    private void StartAim(InputAction.CallbackContext context)
    {
        if (_coroutineAim != null)
        {
            StopCoroutine(_coroutineAim);
        }
        _coroutineAim = StartCoroutine(AimAnimation(true));
    }

    private void EndAim(InputAction.CallbackContext context)
    {
        if (_coroutineAim != null)
        {
            StopCoroutine(_coroutineAim);
        }
        _coroutineAim = StartCoroutine(AimAnimation(false));
    }

    IEnumerator AimAnimation(bool isStarting)
    {
        float _currentTime = 0;
        if (isStarting)
        {
            _isAiming = true;
            _currentSocketPosition = m_aimPosition;
            _lastRotation = m_aimRotation;

            while (_currentTime <= m_startAimTime)
            {
                _socket.localPosition = Vector3.Slerp(_socketOriginalPosition, m_aimPosition, _currentTime / m_startAimTime);
                _socket.localRotation = Quaternion.Slerp(_socketOriginalRotation, m_aimRotation, _currentTime / m_startAimTime);
                _currentTime += Time.deltaTime;
                yield return null;
            }
            _socket.localPosition = m_aimPosition;
            _socket.localRotation = m_aimRotation;
        }
        else
        {
            _currentSocketPosition = _socketOriginalPosition;
            _lastRotation = _socketOriginalRotation;
            while (_currentTime <= m_endAimTime)
            {
                _socket.localPosition = Vector3.Slerp(m_aimPosition, _socketOriginalPosition, _currentTime / m_startAimTime);
                _socket.localRotation = Quaternion.Slerp(m_aimRotation, _socketOriginalRotation, _currentTime / m_startAimTime);
                _currentTime += Time.deltaTime;
                yield return null;
            }
            _isAiming = false;
            _socket.localPosition = _socketOriginalPosition;
            _socket.localRotation = _socketOriginalRotation;
        }
    }

    private void StartDamping()
    {
        if (_coroutineDamping != null)
        {
            StopCoroutine(_coroutineDamping);
        }
        _coroutineDamping = StartCoroutine(DampingAnimation());
    }
    IEnumerator DampingAnimation()
    {
        float _currentTime = 0;
        Vector3 _startPosition = _socket.localPosition;
        while (_currentTime <= m_dampingTime)
        {
            float t = _currentTime / m_dampingTime;
            float offsetY = m_dampingCurve.Evaluate(t);
            _socket.localPosition = _startPosition + new Vector3(0, -offsetY * m_dampingAmplitude, 0);
            _currentTime += Time.deltaTime;
            yield return null;
        }
        _socket.localPosition = _startPosition;
        _coroutineDamping = null;
    }
}
