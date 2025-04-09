using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

//TESTING
public class Weapon : MonoBehaviour
{
    public float m_maxAngleInertia = 30.0f;
    public float m_inertiaSpeed = 5.0f;
    public float m_returnToOriginalPositionInertiaSpeed = 5.0f;

    public float m_recoilUpPositionSpeed = 2.0f;
    public float m_recoilRightRotationSpeed = 5.0f;
    public float m_recoilTime = 0.25f;
    public float m_recoilRecoverySpeedTime = 0.5f;

    private Transform _socket;
    private Quaternion _socketOriginalRotation;
    private Vector3 _socketOriginalPosition;
    private Quaternion _lastRotation;
    private Coroutine _coroutine;
    private bool _isRecoiling = false;
    public AnimationCurve m_recoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private void Start()
    {
        _socket = transform.parent;
        _socketOriginalRotation = _socket.localRotation;
        _socketOriginalPosition = _socket.localPosition;

        GameManager.GetInstance().m_inputManager.m_fire.started += Fire;
    }

    private void Update()
    {
        if (_isRecoiling)
        {
            return;
        }

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
            Quaternion targetRot = Quaternion.Euler(dir.y * m_maxAngleInertia, -dir.x * m_maxAngleInertia, 0f);
            _socket.localRotation = Quaternion.Slerp(_socket.localRotation, _socketOriginalRotation * targetRot, Time.deltaTime * m_inertiaSpeed);
            _lastRotation = _socket.localRotation;
        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(FireAnimation());
    }

    IEnumerator FireAnimation()
    {
        _isRecoiling = true;
        float _currentRecoilSpeedTime = 0;
        float _currentRecoilRecoverySpeedTime = 0;

        while (_currentRecoilSpeedTime <= m_recoilTime)
        {
            float t = _currentRecoilSpeedTime / m_recoilTime;
            float curveT = m_recoilCurve.Evaluate(t);
            _socket.localPosition = Vector3.Slerp(_socketOriginalPosition, _socketOriginalPosition + Vector3.up * m_recoilUpPositionSpeed, curveT);
            Quaternion recoilRot = Quaternion.Euler(m_recoilRightRotationSpeed, 0f, 0f);
            _socket.localRotation = Quaternion.Slerp(_socketOriginalRotation, _socketOriginalRotation * recoilRot, curveT);
            _currentRecoilSpeedTime += Time.deltaTime;
            yield return null;
        }

        while (_currentRecoilRecoverySpeedTime <= m_recoilRecoverySpeedTime)
        {
            _socket.localPosition = Vector3.Slerp(_socketOriginalPosition + Vector3.up * m_recoilUpPositionSpeed, _socketOriginalPosition, _currentRecoilRecoverySpeedTime / m_recoilRecoverySpeedTime);
            _socket.localRotation = Quaternion.Slerp(_socket.localRotation, _socketOriginalRotation, _currentRecoilRecoverySpeedTime / m_recoilRecoverySpeedTime);
            _currentRecoilRecoverySpeedTime += Time.deltaTime;
            yield return null;

        }
        _isRecoiling = false;
        _coroutine = null;
    }
}
