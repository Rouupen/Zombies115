using System.Net.Sockets;
using UnityEngine;

public class HeadShake : MonoBehaviour
{
    public AnimationCurve m_bounceCurve;
    public float m_horizontalWalkingSpeed = 3f;
    public float m_verticalWalkingSpeed = 2f;
    public float m_horizontalAmplitude = 0.03f;
    public float m_verticalAmplitude = 0.02f;
    public float m_inertiaSpeed = 5.0f;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _socketOriginalRotation = transform.localRotation;
        _socketOriginalPosition = transform.localPosition;
        _lastPosition = _socketOriginalPosition;
        _lastRotation = _socketOriginalRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetInstance().m_playerController.m_characterController.isGrounded)
        {
            float velocity = GameManager.GetInstance().m_playerController.m_characterController.velocity.magnitude;

            if (velocity > 0.1f)
            {
                _bobbingTime += Time.deltaTime * m_verticalWalkingSpeed;
                float curveTime = _bobbingTime % 1f; // keep it in 0-1
                float yOffset = m_bounceCurve.Evaluate(curveTime) * m_verticalAmplitude;
                float xOffset = Mathf.Sin(_bobbingTime * m_horizontalWalkingSpeed) * m_horizontalAmplitude;
                //float rotation = Mathf.Sin(m_horizontalWalkingSpeed * Time.time);
                transform.localPosition = _socketOriginalPosition + new Vector3(xOffset, yOffset, 0f);
                transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z + xOffset * 3);
            }
            else
            {
                _bobbingTime = 0f;
                transform.localPosition = Vector3.Slerp(transform.localPosition, _socketOriginalPosition, Time.deltaTime * m_inertiaSpeed);
            }
        }
    }
}
