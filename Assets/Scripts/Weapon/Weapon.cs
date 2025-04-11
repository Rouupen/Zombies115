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
    public float m_inertiaSpeed = 5.0f;
    public float m_returnToOriginalPositionInertiaSpeed = 7.0f;

    [Header("Recoil Inertia")]
    public float m_recoilUpPositionSpeed = 0.05f;
    public float m_recoilRightRotationSpeed = -20f;
    public float m_recoilTime = 0.05f;
    public float m_recoilRecoverySpeedTime = 0.15f;
    public Vector2 m_randomRotationInertia = new Vector2(1,5);
    public AnimationCurve m_recoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Aim")]
    public Vector3 m_aimPosition;
    public Quaternion m_aimRotation;
    public float m_startAimTime = 0.2f;
    public float m_endAimTime = 0.2f;
    public float m_inertiaReductionWhileAiming = 0.1f;


    [Header("Walking")]
    public AnimationCurve m_bounceCurve;
    public float m_horizontalWalkingSpeed = 3f;
    public float m_verticalWalkingSpeed = 2f;
    public float m_horizontalAmplitude = 0.03f;
    public float m_verticalAmplitude = 0.02f;

    [Header("Damping")]
    public AnimationCurve m_dampingCurve;
    public float m_dampingTime = 0.35f;
    public float m_dampingAmplitude = 0.1f;


    private Transform _socket;
    private Quaternion _socketOriginalRotation;
    private Vector3 _socketOriginalPosition;
    private Vector3 _lastPosition;
    private Quaternion _lastRotation;
    private Coroutine _coroutineFire;
    private Coroutine _coroutineAim;
    private Coroutine _coroutineDamping;
    private bool _isRecoiling = false;
    private bool _isAiming = false;
    private float _bobbingTime = 0f;


    //Temp
    bool _wasGrounded = true;
    private void Start()
    {
        _socket = transform.parent;
        _socketOriginalRotation = _socket.localRotation;
        _socketOriginalPosition = _socket.localPosition;
        _lastPosition = _socketOriginalPosition;
        _lastRotation = _socketOriginalRotation;

        GameManager.GetInstance().m_inputManager.m_fire.started += Fire;
        GameManager.GetInstance().m_inputManager.m_aim.started += StartAim;
        GameManager.GetInstance().m_inputManager.m_aim.canceled += EndAim;
    }

    private void Update()
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
        float recoilInertiaReduction = _isAiming ? m_inertiaReductionWhileAiming : 1;

        Vector2 dir = GameManager.GetInstance().m_inputManager.m_look.ReadValue<Vector2>();
        dir.y += GameManager.GetInstance().m_playerController.m_characterController.velocity.normalized.y;
        dir = Vector2.ClampMagnitude(dir, 1f);
        if (dir == Vector2.zero)
        {
            float angleDiff = Quaternion.Angle(_socket.localRotation, _socketOriginalRotation);
            float returnSpeed = (angleDiff / m_maxAngleInertia) * m_returnToOriginalPositionInertiaSpeed;
            _socket.localRotation = Quaternion.Slerp(_socket.localRotation, _socketOriginalRotation, Time.deltaTime * returnSpeed);
        }
        else
        {
            Quaternion targetRot = Quaternion.Euler(dir.y * m_maxAngleInertia * recoilInertiaReduction, -dir.x * m_maxAngleInertia * recoilInertiaReduction, 0f);
            _socket.localRotation = Quaternion.Slerp(_socket.localRotation, _socketOriginalRotation * targetRot, Time.deltaTime * m_inertiaSpeed);
        }

        if (!_isAiming && GameManager.GetInstance().m_playerController.m_characterController.isGrounded)
        {
            float velocity = GameManager.GetInstance().m_playerController.m_characterController.velocity.magnitude;

            if (velocity > 0.1f)
            {
                _bobbingTime += Time.deltaTime * m_verticalWalkingSpeed;
                float curveTime = _bobbingTime % 1f; // keep it in 0-1
                float yOffset = m_bounceCurve.Evaluate(curveTime) * m_verticalAmplitude;
                float xOffset = Mathf.Sin(_bobbingTime * m_horizontalWalkingSpeed) * m_horizontalAmplitude;
                //float rotation = Mathf.Sin(m_horizontalWalkingSpeed * Time.time);
                _socket.localPosition = _socketOriginalPosition + new Vector3(xOffset, yOffset, 0f);
                _socket.localRotation = Quaternion.Euler(_socket.localRotation.eulerAngles.x , _socket.localRotation.eulerAngles.y, _socket.localRotation.eulerAngles.z + xOffset *3);
            }
            else
            {
                _bobbingTime = 0f;
                _socket.localPosition = Vector3.Slerp(_socket.localPosition, _socketOriginalPosition, Time.deltaTime * m_inertiaSpeed);
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
        Vector3 currentPosition = _lastPosition;
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
            _lastPosition = m_aimPosition;
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
            _lastPosition = _socketOriginalPosition;
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
