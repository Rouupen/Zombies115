using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

//TESTING - Spaghetti code for now :)
public class Weapon : MonoBehaviour
{
    [Header("---FAST TESTING---")]
    [Header("Look Inertia")]
    public Vector2 m_maxAngleInertia = new Vector2(30, 15);
    public float m_maxTranslationInertia = 10.0f;
    public float m_lookInertiaRotationSpeed = 5.0f;
    public float m_lookInertiaTraslationSpeed = 5.0f;
    public float m_returnToOriginalPositionInertiaTime = 0.25f;
    public AnimationCurve m_returnToOriginalPositionInertiaCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    //Look Inertia private values
    private Vector3 _positionLookInertia;
    private Quaternion _rotationLookInertia;
    private Coroutine _returnToPositionInertia;

    [Header("Idle")]
    public Vector3 m_localPositionWeaponIdle;
    public Quaternion m_localRotationWeaponIdle;

    [Header("Walking - Start/End")]
    public Vector3 m_localPositionWeaponWalking;
    public Quaternion m_localRotationWeaponWalking;
    public AnimationCurve m_localPositionWalkingTranslationCurve;
    public float m_localPositionWeaponWalkingTime = 0.15f;

    [Header("Walking - Direction translation")]
    public float m_translationSpeed = 1f;
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
    private Vector3 _currentDirectionTranslation;
    private Coroutine _startEndWalking;
    private bool _isWalking;


    [Header("Recoil Inertia")]
    public Vector3 m_maxRecoilRotation = new Vector3(60.0f, 0, 0);
    public Vector3 m_maxRecoilPosition = new Vector3(0, 0.5f, 0);
    public Vector3 m_recoilAdditivePosition = new Vector3(0, 0.05f, 0);
    public Vector3 m_recoilAdditiveRotation = new Vector3(20.0f, 0, 0);
    public float m_recoilStartTime = 0.05f;
    public float m_recoilRecoverySpeedTime = 0.15f;
    public Vector2 m_randomRotationRecoilInertiaY = new Vector2(1, 5);
    public AnimationCurve m_recoilRotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve m_recoilPositionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve m_recoveryRecoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Aim")]
    public Vector3 m_aimPosition;
    public Vector3 m_aimRotation;
    public float m_startAimTime = 0.2f;
    public float m_endAimTime = 0.2f;
    public float m_inertiaReductionWhileAiming = 0.1f;
    public float m_recoilReductionWhileAiming = 0.5f;


    [Header("Damping")]
    public AnimationCurve m_dampingCurve;
    public float m_dampingTime = 0.35f;
    public float m_dampingAmplitude = 0.1f;


    private Transform _socket;
    private Quaternion _socketOriginalRotation;
    private Vector3 _socketOriginalPosition;

    private Coroutine _coroutineFire;
    private Coroutine _coroutineAim;
    private Coroutine _coroutineDamping;
    private bool _isRecoiling = false;
    private bool _isAiming = false;



    //Temp
    bool _wasGrounded = true;
    private Vector3 _startEndWalkingPositionOffset;
    private Quaternion _startEndWalkingRotationOffset = Quaternion.identity;
    private Vector3 _walkingPositionOffset;
    private Quaternion _walkingRotationOffset = Quaternion.identity;
    private Vector3 _inertiaPositionOffset;
    private Quaternion _inertiaRotationOffset = Quaternion.identity;
    private Vector3 _recoilPositionOffset;
    private Quaternion _recoilRotationOffset = Quaternion.identity;
    private Vector3 _aimPositionOffset;
    private Quaternion _aimRotationOffset = Quaternion.identity;
    private Vector3 _dampingPositionOffset;
    private Quaternion _dampingRotationOffset = Quaternion.identity;

    private Vector3 _currentSocketPosition;
    private Quaternion _currentSocketRotation;

    private void Start()
    {
        _socket = transform.parent;
        _socketOriginalRotation = _socket.localRotation;
        _socketOriginalPosition = _socket.localPosition;
        _currentSocketPosition = m_localPositionWeaponIdle;
        _currentSocketRotation = m_localRotationWeaponIdle;
        _walkingRotationOffset = Quaternion.identity;
        GameManager.GetInstance().m_inputManager.m_fire.started += Fire;
        GameManager.GetInstance().m_inputManager.m_aim.started += StartAim;
        GameManager.GetInstance().m_inputManager.m_aim.canceled += EndAim;
    }

    private void LateUpdate()
    {
        //Ground damping - Temp
        if (!_wasGrounded && GameManager.GetInstance().m_playerController.m_characterController.isGrounded)
        {
            StartDamping();
        }
        _wasGrounded = GameManager.GetInstance().m_playerController.m_characterController.isGrounded;

        UpdateInertia();
        WalkingLoop();

        _socket.localPosition = _currentSocketPosition + _inertiaPositionOffset + _walkingPositionOffset + _recoilPositionOffset + _aimPositionOffset + _dampingPositionOffset;
        _socket.localRotation = _currentSocketRotation * _inertiaRotationOffset * _walkingRotationOffset * _recoilRotationOffset * _aimRotationOffset * _dampingRotationOffset;
    }
    private Vector2 _lookVelocitySmooth = Vector2.zero;
    private Vector2 _lookVelocityRef = Vector2.zero;

    private void ReturnToPositionInertiaAnimation()
    {
        if (_returnToPositionInertia != null)
        {
            StopCoroutine(_returnToPositionInertia);
        }
        _returnToPositionInertia = StartCoroutine(ReturnToPositionInertia());
    }

    private IEnumerator ReturnToPositionInertia()
    {
        float currentTime = 0;

        Vector3 originalPositionOffset = _inertiaPositionOffset;
        Quaternion originalRotaitonOffset = _inertiaRotationOffset;

        while (currentTime <= m_returnToOriginalPositionInertiaTime)
        {
            float tValue = m_returnToOriginalPositionInertiaCurve.Evaluate(currentTime / m_returnToOriginalPositionInertiaTime);
            _inertiaRotationOffset = Quaternion.Slerp(originalRotaitonOffset, Quaternion.identity, tValue);
            _inertiaPositionOffset = Vector3.Slerp(originalPositionOffset, Vector3.zero, tValue);
            currentTime += Time.deltaTime;
            yield return null;
        }
        _inertiaRotationOffset = Quaternion.identity;
        _inertiaPositionOffset = Vector3.zero;
        _returnToPositionInertia = null;
    }
    private void UpdateInertia()
    {
        float recoilInertiaReduction = _isAiming ? m_inertiaReductionWhileAiming : 1;

        Vector2 lookVelocity = GameManager.GetInstance().m_playerController.m_characterLook.GetLookVelocity();
        lookVelocity.y += GameManager.GetInstance().m_playerController.m_characterController.velocity.y;
        if (lookVelocity.magnitude <= 0.1f)
        {
            if (_returnToPositionInertia == null && (_inertiaRotationOffset != Quaternion.identity || _inertiaPositionOffset != Vector3.zero))
            {
                ReturnToPositionInertiaAnimation();
            }
        }
        else
        {
            Vector2 rawLookVelocity = GameManager.GetInstance().m_playerController.m_characterLook.GetLookVelocity();
            _lookVelocitySmooth = Vector2.SmoothDamp(_lookVelocitySmooth, rawLookVelocity, ref _lookVelocityRef, 0.1f); // 0.01s smooth
            lookVelocity = _lookVelocitySmooth;

            if (_returnToPositionInertia != null)
            {
                StopCoroutine(_returnToPositionInertia);
                _returnToPositionInertia = null;
            }

            Quaternion targetRot = Quaternion.Euler(
                Mathf.Clamp(-lookVelocity.y * m_lookInertiaRotationSpeed * recoilInertiaReduction, -m_maxAngleInertia.y, m_maxAngleInertia.y),
                Mathf.Clamp(lookVelocity.x * m_lookInertiaRotationSpeed * recoilInertiaReduction, -m_maxAngleInertia.x, m_maxAngleInertia.x),
                0f);

            Vector3 targetPos = new Vector3(
                Mathf.Clamp(-lookVelocity.x * m_lookInertiaTraslationSpeed * recoilInertiaReduction, -m_maxTranslationInertia, m_maxTranslationInertia),
                0f,
                0f);

            if (_isAiming)
            {
                targetPos = Vector3.zero;
            }

            _inertiaPositionOffset = Vector3.Slerp(_inertiaPositionOffset, targetPos, Time.deltaTime * m_lookInertiaTraslationSpeed);
            _inertiaRotationOffset = Quaternion.Slerp(_inertiaRotationOffset, targetRot, Time.deltaTime * m_lookInertiaRotationSpeed);
        }
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
        Vector3 startPosition = starting ? _walkingPositionOffset : _walkingPositionOffset;
        Vector3 endPosition = starting ? m_localPositionWeaponWalking - m_localPositionWeaponIdle : Vector3.zero;

        Quaternion startRotation = starting ? Quaternion.identity : m_localRotationWeaponWalking;
        Quaternion endRotation = starting ? m_localRotationWeaponWalking : Quaternion.identity;
        while (currentTime <= m_localPositionWeaponWalkingTime)
        {
            float valueCurve = m_localPositionWalkingTranslationCurve.Evaluate((currentTime / m_localPositionWeaponWalkingTime) % 1);
            print(valueCurve);
            Vector3 currentPosition = Vector3.Slerp(startPosition, endPosition, valueCurve);
            Quaternion currentRotation = Quaternion.Slerp(startRotation, endRotation, valueCurve);

            _startEndWalkingPositionOffset = currentPosition;
            _startEndWalkingRotationOffset = currentRotation;

            currentTime += Time.deltaTime;
            yield return null;
        }
        _startEndWalkingPositionOffset = endPosition;
        _startEndWalkingRotationOffset = endRotation;

        _startEndWalking = null;
    }
    private void WalkingLoop()
    {
        //WALKING
        if (GameManager.GetInstance().m_playerController.m_characterController.isGrounded)
        {
            float recoilInertiaReduction = _isAiming ? m_inertiaReductionWhileAiming : 1;

            Vector3 characterVelocity = GameManager.GetInstance().m_playerController.m_characterController.velocity;
            characterVelocity.y = 0f; //Ingore the Y-axies

            if (characterVelocity.magnitude > 0.1f && !_isAiming)
            {
                //Temp - Need state machine
                if (!_isWalking)
                {
                    StartEndWalking(true);
                }

                //Bounce
                _currentVerticalBounceTime += Time.deltaTime / m_bounceVerticalTime;
                _currentHorizontalBounceTime += Time.deltaTime / m_bounceHorizontalTime;
                _currentForwardRotationTime += Time.deltaTime / m_rotationLoopForwardTime;

                Vector3 newPosition = new Vector3(
                    m_bounceHorizontalCurve.Evaluate(_currentHorizontalBounceTime % 1) * m_bounceHorizontalAmplitude * recoilInertiaReduction,
                    m_bounceVerticalCurve.Evaluate(_currentVerticalBounceTime % 1) * m_bounceVerticalAmplitude * recoilInertiaReduction,
                    0.0f
                    );

                Quaternion newRotation = Quaternion.Euler(
                    0.0f,
                    0.0f,
                    m_rotationLoopForwardCurve.Evaluate(_currentForwardRotationTime % 1) * m_rotationForwardAmplitude * recoilInertiaReduction
                    );


                //Dir translation
                Vector3 charDirection = GameManager.GetInstance().m_inputManager.m_move.ReadValue<Vector2>();
                charDirection.x *= m_translationHorizontalDistance * recoilInertiaReduction;
                charDirection.z = -charDirection.y * m_translationForwardDistance * recoilInertiaReduction;
                charDirection.y = 0;

                _currentDirectionTranslation += charDirection * m_translationSpeed * Time.deltaTime;

                _currentDirectionTranslation.x = Mathf.Clamp(_currentDirectionTranslation.x, -m_translationHorizontalDistance, m_translationHorizontalDistance);
                _currentDirectionTranslation.z = Mathf.Clamp(_currentDirectionTranslation.z, -m_translationForwardDistance, m_translationForwardDistance);

                _walkingPositionOffset = newPosition + _currentDirectionTranslation + _startEndWalkingPositionOffset;
                _walkingRotationOffset = newRotation * _startEndWalkingRotationOffset;
            }
            else
            {
                if (_isWalking)
                {
                    StartEndWalking(false);
                }
                _walkingPositionOffset = _startEndWalkingPositionOffset;
                _walkingRotationOffset = _startEndWalkingRotationOffset;
                _currentVerticalBounceTime = 0f;
                _currentHorizontalBounceTime = 0f;
                _currentForwardRotationTime = 0f;
                _currentDirectionTranslation = Vector3.zero;
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
        float recoilInertiaReduction = _isAiming ? m_inertiaReductionWhileAiming : 1;
        float recoilReductionWhileAiming = _isAiming ? m_recoilReductionWhileAiming : 1;
        _isRecoiling = true;
        float _currentRecoilSpeedTime = 0;
        float _currentRecoilRecoverySpeedTime = 0;

        Vector3 startPosition = _recoilPositionOffset;
        Vector3 endPosition = _recoilPositionOffset + m_recoilAdditivePosition * recoilInertiaReduction;
        endPosition.x = Mathf.Clamp(endPosition.x, -m_maxRecoilPosition.x * recoilInertiaReduction, m_maxRecoilPosition.x * recoilInertiaReduction);
        endPosition.y = Mathf.Clamp(endPosition.y, -m_maxRecoilPosition.y * recoilInertiaReduction, m_maxRecoilPosition.y * recoilInertiaReduction);
        endPosition.z = Mathf.Clamp(endPosition.z, -m_maxRecoilPosition.z * recoilInertiaReduction, m_maxRecoilPosition.z * recoilInertiaReduction);

        Vector3 randomRotation = new Vector3(UnityEngine.Random.Range(-1, 1.0f), UnityEngine.Random.Range(-1, 1.0f), 0) * UnityEngine.Random.Range(m_randomRotationRecoilInertiaY.x, m_randomRotationRecoilInertiaY.y);
        Quaternion startRotation = _recoilRotationOffset;
        Vector3 endRotationClamped = (_recoilRotationOffset.eulerAngles + (m_recoilAdditiveRotation + randomRotation) * recoilReductionWhileAiming);

        endRotationClamped.x = Mathf.Clamp(endRotationClamped.x > 180 ? endRotationClamped.x - 360 : endRotationClamped.x, -m_maxRecoilRotation.x * recoilInertiaReduction, m_maxRecoilRotation.x * recoilInertiaReduction);
        endRotationClamped.y = Mathf.Clamp(endRotationClamped.y > 180 ? endRotationClamped.y - 360 : endRotationClamped.y, -m_maxRecoilRotation.y * recoilInertiaReduction, m_maxRecoilRotation.y * recoilInertiaReduction);
        endRotationClamped.z = Mathf.Clamp(endRotationClamped.z > 180 ? endRotationClamped.z - 360 : endRotationClamped.z, -m_maxRecoilRotation.z * recoilInertiaReduction, m_maxRecoilRotation.z * recoilInertiaReduction);
        Quaternion endRotation = Quaternion.Euler(endRotationClamped);

        while (_currentRecoilSpeedTime <= m_recoilStartTime)
        {
            float curveRotationT = m_recoilRotationCurve.Evaluate(_currentRecoilSpeedTime / m_recoilStartTime);
            float curvePositionT = m_recoilPositionCurve.Evaluate(_currentRecoilSpeedTime / m_recoilStartTime);

            _recoilPositionOffset = Vector3.Slerp(startPosition, endPosition, curvePositionT);
            _recoilRotationOffset = Quaternion.Slerp(startRotation, endRotation, curveRotationT);

            _currentRecoilSpeedTime += Time.deltaTime;
            yield return null;
        }

        while (_currentRecoilRecoverySpeedTime <= m_recoilRecoverySpeedTime)
        {
            float curveT = m_recoveryRecoilCurve.Evaluate(_currentRecoilRecoverySpeedTime / m_recoilRecoverySpeedTime);
            _recoilPositionOffset = Vector3.Slerp(endPosition, Vector3.zero, curveT);
            _recoilRotationOffset = Quaternion.Slerp(endRotation, Quaternion.identity, curveT);

            _currentRecoilRecoverySpeedTime += Time.deltaTime;
            yield return null;

        }

        _recoilPositionOffset = Vector3.zero;
        _recoilRotationOffset = Quaternion.identity;
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
        _isAiming = isStarting;

        float _currentTime = 0;
        Vector3 startPosition = _aimPositionOffset;
        Quaternion startRotation = _aimRotationOffset;

        Vector3 endPosition = isStarting ? m_aimPosition - _currentSocketPosition : Vector3.zero;
        Quaternion endRotation = isStarting ? Quaternion.Euler(m_aimRotation - _currentSocketRotation.eulerAngles) : Quaternion.identity;

        while (_currentTime <= m_startAimTime)
        {
            _aimPositionOffset = Vector3.Slerp(startPosition, endPosition, _currentTime / m_startAimTime);
            _aimRotationOffset = Quaternion.Slerp(startRotation, endRotation, _currentTime / m_startAimTime);
            _currentTime += Time.deltaTime;
            yield return null;
        }

        _aimPositionOffset = endPosition;
        _aimRotationOffset = endRotation;
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
        Vector3 startPosition = Vector3.zero;

        Vector3 endPosition = GameManager.GetInstance().m_playerController.m_characterController.velocity;
        endPosition.x /= 2;
        endPosition.z = 0;
        endPosition = endPosition.normalized * m_dampingAmplitude;

        while (_currentTime <= m_dampingTime)
        {
            float valueT = m_dampingCurve.Evaluate(_currentTime / m_dampingTime);
            _dampingPositionOffset = Vector3.Slerp(startPosition, endPosition, valueT);
            _currentTime += Time.deltaTime;
            yield return null;
        }
        _dampingPositionOffset = Vector3.zero;
        _coroutineDamping = null;
    }
}
