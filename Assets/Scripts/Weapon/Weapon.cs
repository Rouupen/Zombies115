using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

//TESTING - Spaghetti code for now :)
public class Weapon : MonoBehaviour
{
    [Header("---FAST TESTING---")]
    [Header("Look Inertia")]
    public Vector2 m_maxAngleInertia = new Vector2(30,15);
    public float m_maxTranslationInertia = 10.0f;
    public float m_lookInertiaRotationSpeed = 5.0f;
    public float m_lookInertiaTraslationSpeed = 5.0f;
    public float m_returnToOriginalPositionInertiaTime = 0.25f;
    public AnimationCurve m_returnToOriginalPositionInertiaCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    //Look Inertia private values
    private Vector3 _positionLookInertia;
    private Quaternion _rotationLookInertia;
    private Coroutine _returnToPositionInertia;


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
    private Vector3 _currentDirecitionTranslation;
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
    private Quaternion _currentSocketRotation;
    private Coroutine _coroutineFire;
    private Coroutine _coroutineAim;
    private Coroutine _coroutineDamping;
    private bool _isRecoiling = false;
    private bool _isAiming = false;



    //Temp
    bool _wasGrounded = true;
    private Vector3 _walkingPositionOffset;
    private Quaternion _walkingRotationOffset;
    private Vector3 _inertiaPositionOffset;
    private Quaternion _inertiaRotationOffset;

    private void Start()
    {
        _socket = transform.parent;
        _socketOriginalRotation = _socket.localRotation;
        _socketOriginalPosition = _socket.localPosition;
        _currentSocketPosition = _socketOriginalPosition;
        _currentSocketRotation = _socketOriginalRotation;
        _walkingRotationOffset = Quaternion.identity;
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
        UpdateInertia();
        WalkingLoop();

        _socket.localPosition = _currentSocketPosition + _inertiaPositionOffset + _walkingPositionOffset;
        _socket.localRotation = _currentSocketRotation * _inertiaRotationOffset * _walkingRotationOffset;
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
        Vector3 endPosition = starting ? m_localPositionWeaponWalking : m_localPositionWeaponIdle;
        Quaternion endRotation = starting ? m_localRotationWeaponWalking : m_localRotationWeaponIdle;

        Vector3 startPosition = _socket.localPosition;
        Quaternion startRotation = _socket.localRotation;
        while (currentTime <= m_localPositionWeaponWalkingTime)
        {
            float valueCurve = m_localPositionWalkingTranslationCurve.Evaluate((currentTime / m_localPositionWeaponWalkingTime) % 1);
            print(valueCurve);
            Vector3 currentPosition = Vector3.Slerp(startPosition, endPosition, valueCurve);
            Quaternion currentRotation = Quaternion.Slerp(startRotation, endRotation, valueCurve);

            _currentSocketPosition = currentPosition;
            _currentSocketRotation = currentRotation;

            currentTime += Time.deltaTime;
            yield return null;
        }
        _currentSocketPosition = endPosition;
        _currentSocketRotation = endRotation;
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

                //Bounce
                _currentVerticalBounceTime += Time.deltaTime / m_bounceVerticalTime;
                _currentHorizontalBounceTime += Time.deltaTime / m_bounceHorizontalTime;
                _currentForwardRotationTime += Time.deltaTime / m_rotationLoopForwardTime;

                Vector3 newPosition = new Vector3(
                    m_bounceHorizontalCurve.Evaluate(_currentHorizontalBounceTime % 1) * m_bounceHorizontalAmplitude,
                    m_bounceVerticalCurve.Evaluate(_currentVerticalBounceTime % 1) * m_bounceVerticalAmplitude,
                    0.0f
                    );

                Quaternion newRotation = Quaternion.Euler(
                    0.0f,
                    0.0f,
                    m_rotationLoopForwardCurve.Evaluate(_currentForwardRotationTime % 1) * m_rotationForwardAmplitude
                    );


                //Dir translation
                Vector3 charDirection = GameManager.GetInstance().m_inputManager.m_move.ReadValue<Vector2>();
                charDirection.x *= m_translationHorizontalDistance;
                charDirection.z = -charDirection.y * m_translationForwardDistance;
                charDirection.y = 0;

                _currentDirecitionTranslation += charDirection * m_translationSpeed * Time.deltaTime;

                _currentDirecitionTranslation.x = Mathf.Clamp(_currentDirecitionTranslation.x, -m_translationHorizontalDistance, m_translationHorizontalDistance);
                _currentDirecitionTranslation.z = Mathf.Clamp(_currentDirecitionTranslation.z, -m_translationForwardDistance, m_translationForwardDistance);

                _walkingPositionOffset = newPosition + _currentDirecitionTranslation;
                _walkingRotationOffset = newRotation;

                //Move and rotate
                //_socket.localPosition = _currentSocketPosition + newPosition + _currentDirecitionTranslation;
                //_socket.localRotation = _currentSocketRotation * newRotation;
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
                _currentDirecitionTranslation = Vector3.zero;
            }

        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        //if (_coroutineFire != null)
        //{
        //    StopCoroutine(_coroutineFire);
        //}
        //_coroutineFire = StartCoroutine(FireAnimation());
    }

    IEnumerator FireAnimation()
    {
        _isRecoiling = true;
        float _currentRecoilSpeedTime = 0;
        float _currentRecoilRecoverySpeedTime = 0;
        Vector3 currentPosition = _currentSocketPosition;
        Quaternion currentRotation = _socket.localRotation;
        currentRotation.y = _currentSocketRotation.y;
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
        //if (_coroutineAim != null)
        //{
        //    StopCoroutine(_coroutineAim);
        //}
        //_coroutineAim = StartCoroutine(AimAnimation(true));
    }

    private void EndAim(InputAction.CallbackContext context)
    {
        //if (_coroutineAim != null)
        //{
        //    StopCoroutine(_coroutineAim);
        //}
        //_coroutineAim = StartCoroutine(AimAnimation(false));
    }

    IEnumerator AimAnimation(bool isStarting)
    {
        float _currentTime = 0;
        if (isStarting)
        {
            _isAiming = true;
            _currentSocketPosition = m_aimPosition;
            _currentSocketRotation = m_aimRotation;

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
            _currentSocketRotation = _socketOriginalRotation;
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
        //if (_coroutineDamping != null)
        //{
        //    StopCoroutine(_coroutineDamping);
        //}
        //_coroutineDamping = StartCoroutine(DampingAnimation());
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
