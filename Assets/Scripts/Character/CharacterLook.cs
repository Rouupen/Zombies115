using System.Collections;
using UnityEngine;

public class CharacterLook : MonoBehaviour
{
    [Header("Values")]
    public float m_lookSpeed = 5.0f;

    [Header("Camera Shake")]
    public float m_shakeDuration = 0.2f;
    public float m_shakeMagnitude = 0.1f;


    private float xRotation = 0;
    private Vector2 _lookVelocity;
    private float _shakeTimer = 0.0f;
    private Vector3 _initialLocalPos;
    private Vector3 _currentSocketPosition;
    private Quaternion _currentSocketRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _initialLocalPos = transform.localPosition;
        _currentSocketPosition = transform.localPosition;
        _currentSocketRotation = transform.localRotation;
    }

    void Update()
    {
        //Temp - Rotation
        transform.parent.Rotate(Vector3.up, GetLookInput().x * m_lookSpeed * Time.deltaTime);
        xRotation -= GetLookInput().y * m_lookSpeed * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        _lookVelocity = new Vector2(GetLookInput().x * m_lookSpeed * Time.deltaTime, GetLookInput().y * m_lookSpeed * Time.deltaTime);
        Quaternion rotation = Quaternion.Euler(xRotation, 0, 0);
        
        WalkingLoop();

        // Camera shake
        if (_shakeTimer > 0)
        {
            transform.localPosition = _initialLocalPos + Random.insideUnitSphere * m_shakeMagnitude;
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = _initialLocalPos;
        }

        transform.localPosition = _currentSocketPosition + _walkingPositionOffset;
        transform.localRotation = _currentSocketRotation * _walkingRotationOffset * rotation;
    }

    Vector2 GetLookInput()
    {
        //Temp - Read "Look" input value
        //Screen size dependent - CHANGE!!
        Vector2 rawInput = GameManager.GetInstance().m_inputManager.m_look.ReadValue<Vector2>();
        return rawInput;
    }

    public Vector2 GetLookVelocity()
    {
        return _lookVelocity;
    }

    public void ShakeCamera()
    {
        _shakeTimer = m_shakeDuration;
    }


    [Space(10)]
    [Header("       ---WALKING START/END---")]
    [Tooltip("LOCAL position of the weapon while walking")]
    public Vector3 m_walkingLocalPosition;
    [Tooltip("LOCAL rotation of the weapon while walking")]
    public Vector3 m_walkingLocalRotation;
    [Tooltip("Animation curve for moving and rotating the weapon when entering or exiting walking")]
    public AnimationCurve m_walkingPositionAndRotationCurve;
    [Tooltip("Time it takes to move and rotate the weapon when starting or ending walking")]
    public float m_walkingPositionAndRotationTime = 0.15f;

    [Space(10)]
    [Header("       ---WALKING OFFSET---")]
    [Tooltip("Speed at which the weapon offset is applied while walking")]
    public float m_walkingOffsetPositionSpeed = 1f;
    [Tooltip("Maximum horizontal offset distance of the weapon while walking")]
    public float m_walkingOffsetHorizontalDistance = 0.5f;
    [Tooltip("Maximum forward offset distance of the weapon while walking")]
    public float m_walkingOffsetForwardDistance = 0.1f;
    [Tooltip("Time it takes for the walking offset to be applied")]
    public float m_walkingOffsetTime = 0.25f;

    [Space(10)]
    [Header("       ---WALKING LOOP---")]
    [Tooltip("Time for one vertical bounce cycle")]
    public float m_walkingBounceVerticalTime = 0.5f;
    [Tooltip("Vertical bounce amplitude of the weapon while walking")]
    public float m_walkingBounceVerticalAmplitude = 0.02f;
    [Tooltip("Vertical bounce curve of the weapon while walking")]
    public AnimationCurve m_walkingBounceVerticalCurve;
    [Space(8)]
    [Tooltip("Time for one horizontal bounce cycle")]
    public float m_walkingBounceHorizontalTime = 1f;
    [Tooltip("Horizontal bounce amplitude of the weapon while walking")]
    public float m_walkingBounceHorizontalAmplitude = 0.03f;
    [Tooltip("Horizontal bounce curve of the weapon while walking")]
    public AnimationCurve m_walkingBounceHorizontalCurve;
    [Space(8)]
    [Tooltip("Time for one forward rotation cycle")]
    public float m_walkingRotationLoopForwardTime = 1f;
    [Tooltip("Forward rotation amplitude of the weapon while walking")]
    public float m_walkingRotationForwardAngle = 2f;
    [Tooltip("Forward rotation curve of the weapon in a loop while walking")]
    public AnimationCurve m_walkingRotationLoopForwardCurve;


    private float _currentVerticalBounceTime = 0f;
    private float _currentHorizontalBounceTime = 0f;
    private float _currentForwardRotationTime = 0f;
    private Vector3 _currentDirectionTranslation;
    private Coroutine _startEndWalking;
    private bool _isWalking;
    private Vector3 _startEndWalkingPositionOffset;
    private Quaternion _startEndWalkingRotationOffset = Quaternion.identity;
    private Vector3 _walkingPositionOffset;
    private Quaternion _walkingRotationOffset = Quaternion.identity;

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
        Vector3 endPosition = starting ? m_walkingLocalPosition /*- m_idleLocalPosition*/ : Vector3.zero;

        Quaternion startRotation = starting ? Quaternion.identity : Quaternion.Euler(m_walkingLocalRotation);
        Quaternion endRotation = starting ? Quaternion.Euler(m_walkingLocalRotation) : Quaternion.identity;
        while (currentTime <= m_walkingPositionAndRotationTime)
        {
            float valueCurve = m_walkingPositionAndRotationCurve.Evaluate((currentTime / m_walkingPositionAndRotationTime) % 1);
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
                _currentVerticalBounceTime += Time.deltaTime / m_walkingBounceVerticalTime;
                _currentHorizontalBounceTime += Time.deltaTime / m_walkingBounceHorizontalTime;
                _currentForwardRotationTime += Time.deltaTime / m_walkingRotationLoopForwardTime;

                Vector3 newPosition = new Vector3(
                    m_walkingBounceHorizontalCurve.Evaluate(_currentHorizontalBounceTime % 1) * m_walkingBounceHorizontalAmplitude,
                    m_walkingBounceVerticalCurve.Evaluate(_currentVerticalBounceTime % 1) * m_walkingBounceVerticalAmplitude,
                    0.0f
                    );

                Quaternion newRotation = Quaternion.Euler(
                    0.0f,
                    0.0f,
                    m_walkingRotationLoopForwardCurve.Evaluate(_currentForwardRotationTime % 1) * m_walkingRotationForwardAngle
                    );


                //Dir translation
                Vector3 charDirection = GameManager.GetInstance().m_inputManager.m_move.ReadValue<Vector2>();
                charDirection.x *= m_walkingOffsetHorizontalDistance;
                charDirection.z = -charDirection.y * m_walkingOffsetForwardDistance;
                charDirection.y = 0;

                _currentDirectionTranslation += charDirection * m_walkingOffsetPositionSpeed * Time.deltaTime;

                _currentDirectionTranslation.x = Mathf.Clamp(_currentDirectionTranslation.x, -m_walkingOffsetHorizontalDistance, m_walkingOffsetHorizontalDistance);
                _currentDirectionTranslation.z = Mathf.Clamp(_currentDirectionTranslation.z, -m_walkingOffsetForwardDistance, m_walkingOffsetForwardDistance);

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
}