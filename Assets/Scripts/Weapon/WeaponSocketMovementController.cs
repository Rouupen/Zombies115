using System.Collections;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class WeaponMovementData
{
    [Header("       ---INFO---")]
    [ReadOnlyTextArea]
    [SerializeField]
    private string _info = "This section controls everything related to weapon movement. Some values, like recoil time, depend on the weapon's stats." +
       "All variables have descriptions when you hover over them.";

    [Space(10)]
    [Header("       ---LOOK - MOVEMENT CAUSED BY CAMERA ROTATION---")]
    [Tooltip("Maximum angle the weapon will rotate horizontally and vertically")]
    public Vector2 m_lookMaxAngleRotation = new Vector2(30, 15);
    [Tooltip("Speed at which the weapon rotates based on camera movement")]
    public float m_lookAngleRotationSpeed = 5.0f;
    [Tooltip("Maximum horizontal distance the weapon will travel based on where the player looks")]
    public float m_lookMaxHorizontalOffset = 10.0f;
    [Tooltip("Speed of the weapon's horizontal movement based on camera rotation")]
    public float m_lookHorizontalOffsetSpeed = 5.0f;
    [Tooltip("Time it takes for the weapon to return to its original position when the camera stops moving")]
    public float m_lookReturnToOriginalPositionTime = 0.25f;
    [Tooltip("Animation curve used when the weapon returns to its original position")]
    public AnimationCurve m_lookReturnToOriginalPositionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Space(10)]
    [Header("       ---IDLE---")]
    [Tooltip("LOCAL position of the weapon when the character is idle")]
    public Vector3 m_idleLocalPosition;
    [Tooltip("LOCAL rotation of the weapon when the character is idle")]
    public Vector3 m_idleLocalRotation;

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

    [Space(10)]
    [Header("       ---RECOIL---")]
    [Tooltip("Maximum rotation of the weapon due to recoil")]
    public Vector3 m_recoilMaxRotation = new Vector3(60.0f, 0, 0);
    [Tooltip("Additive rotation offset applied during recoil")]
    public Vector3 m_recoilAdditiveRotation = new Vector3(20.0f, 0, 0);
    [Tooltip("Random Y-axis (horizontal) variation range for recoil inertia")]
    public Vector2 m_recoilRandomRotationY = new Vector2(1, 5);
    [Tooltip("Animation curve used for recoil rotation")]
    public AnimationCurve m_recoilRotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Space(8)]
    [Tooltip("Maximum position shift of the weapon due to recoil")]
    public Vector3 m_recoilMaxPosition = new Vector3(0, 0.5f, 0);
    [Tooltip("Additive position offset applied during recoil")]
    public Vector3 m_recoilAdditivePosition = new Vector3(0, 0.05f, 0);
    [Tooltip("Animation curve used for recoil position")]
    public AnimationCurve m_recoilPositionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Space(8)]
    [Tooltip("Time it takes to start the recoil after firing")]
    public float m_recoilStartTime = 0.05f;
    [Tooltip("Speed at which the weapon recovers from recoil")]
    public float m_recoilRecoverySpeedTime = 0.15f;
    [Tooltip("Recovery animation curve after recoil")]
    public AnimationCurve m_recoveryRecoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Space(10)]
    [Header("       ---AIM---")]
    [Tooltip("Weapon position while aiming")]
    public Vector3 m_aimPosition;
    [Tooltip("Weapon rotation while aiming")]
    public Vector3 m_aimRotation;
    [Space(8)]
    [Tooltip("Time it takes to enter aiming mode")]
    public float m_aimStartTime = 0.2f;
    [Tooltip("Time it takes to exit aiming mode")]
    public float m_aimEndTime = 0.2f;
    [Space(8)]
    [Tooltip("Reduction of inertia while aiming")]
    public float m_lookReductionWhileAiming = 0.1f;
    [Tooltip("Reduction of recoil while aiming")]
    public float m_recoilReductionWhileAiming = 0.5f;

    [Space(10)]
    [Header("       ---DAMPING---")]
    [Tooltip("General damping curve applied to weapon movement")]
    public AnimationCurve m_dampingCurve;
    [Tooltip("Time over which damping is applied")]
    public float m_dampingTime = 0.35f;
    [Tooltip("Amplitude of the damping effect")]
    public float m_dampingAmplitude = 0.1f;
}


public class WeaponSocketMovementController : MonoBehaviour
{
    [Header("---You can edit these values at runtime for configuration, but they won't be saved---")]
    [SerializeField]
    public WeaponMovementData m_weaponMovementData;

    //Look private variables
    private Vector3 _lookPosition;
    private Quaternion _lookRotation;
    private Coroutine _lookReturnToPositionCoroutine;

    //Walking private values
    private float _currentVerticalBounceTime = 0f;
    private float _currentHorizontalBounceTime = 0f;
    private float _currentForwardRotationTime = 0f;
    private Vector3 _currentDirectionTranslation;
    private Coroutine _startEndWalking;
    private bool _isWalking;

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

    private Vector2 _lookVelocitySmooth = Vector2.zero;
    private Vector2 _lookVelocityRef = Vector2.zero;

    private Quaternion _socketOriginalRotation;
    private Vector3 _socketOriginalPosition;

    private Coroutine _coroutineFire;
    private Coroutine _coroutineAim;
    private Coroutine _coroutineDamping;
    
    private bool _isAiming = false;
    private bool _canAim = true;

    public void Initialize(WeaponMovementData weaponMovementData)
    {
        m_weaponMovementData = weaponMovementData;

        _socketOriginalRotation = transform.localRotation;
        _socketOriginalPosition = transform.localPosition;
        _currentSocketPosition = m_weaponMovementData.m_idleLocalPosition;
        _currentSocketRotation = Quaternion.Euler(m_weaponMovementData.m_idleLocalRotation);
        _walkingRotationOffset = Quaternion.identity;


        GameManager.GetInstance().m_inputManager.m_aim.started += StartAim;
        GameManager.GetInstance().m_inputManager.m_aim.canceled += EndAim;
    }
    private void OnDestroy()
    {
        GameManager.GetInstance().m_inputManager.m_aim.started -= StartAim;
        GameManager.GetInstance().m_inputManager.m_aim.canceled -= EndAim;
    }
    //Temp - for reset local variables
    public void Deinitialize()
    {
        _lookPosition = Vector3.zero;
        _lookRotation = Quaternion.identity;

        _currentVerticalBounceTime = 0f;
        _currentHorizontalBounceTime = 0f;
        _currentForwardRotationTime = 0f;
        _currentDirectionTranslation = Vector3.zero;

        _isWalking = false;

        _wasGrounded = true;
        _startEndWalkingPositionOffset = Vector3.zero;
        _startEndWalkingRotationOffset = Quaternion.identity;
        _walkingPositionOffset = Vector3.zero;
        _walkingRotationOffset = Quaternion.identity;
        _inertiaPositionOffset = Vector3.zero;
        _inertiaRotationOffset = Quaternion.identity;
        _recoilPositionOffset = Vector3.zero;
        _recoilRotationOffset = Quaternion.identity;
        _aimPositionOffset = Vector3.zero;
        _aimRotationOffset = Quaternion.identity;
        _dampingPositionOffset = Vector3.zero;
        _dampingRotationOffset = Quaternion.identity;

        _currentSocketPosition = Vector3.zero;
        _currentSocketRotation = Quaternion.identity;

        _lookVelocitySmooth = Vector2.zero;
        _lookVelocityRef = Vector2.zero;

        _socketOriginalRotation = Quaternion.identity;
        _socketOriginalPosition = Vector3.zero;

        _isAiming = false;
    }

    private void LateUpdate()
    {
        //Ground damping - Temp
        if (!_wasGrounded && GameManager.GetInstance().m_playerController.m_characterController.isGrounded)
        {
            StartDamping();
        }
        _wasGrounded = GameManager.GetInstance().m_playerController.m_characterController.isGrounded;

        UpdateLook();
        WalkingLoop();

        transform.localPosition = _currentSocketPosition + _inertiaPositionOffset + _walkingPositionOffset + _recoilPositionOffset + _aimPositionOffset + _dampingPositionOffset;
        transform.localRotation = _currentSocketRotation * _inertiaRotationOffset * _walkingRotationOffset * _recoilRotationOffset * _aimRotationOffset * _dampingRotationOffset;
    }


    private void ReturnToPositionInertiaAnimation()
    {
        if (_lookReturnToPositionCoroutine != null)
        {
            StopCoroutine(_lookReturnToPositionCoroutine);
        }
        _lookReturnToPositionCoroutine = StartCoroutine(ReturnToPositionInertia());
    }

    private IEnumerator ReturnToPositionInertia()
    {
        float currentTime = 0;

        Vector3 originalPositionOffset = _inertiaPositionOffset;
        Quaternion originalRotaitonOffset = _inertiaRotationOffset;

        while (currentTime <= m_weaponMovementData.m_lookReturnToOriginalPositionTime)
        {
            float tValue = m_weaponMovementData.m_lookReturnToOriginalPositionCurve.Evaluate(currentTime / m_weaponMovementData.m_lookReturnToOriginalPositionTime);
            _inertiaRotationOffset = Quaternion.Slerp(originalRotaitonOffset, Quaternion.identity, tValue);
            _inertiaPositionOffset = Vector3.Slerp(originalPositionOffset, Vector3.zero, tValue);
            currentTime += Time.deltaTime;
            yield return null;
        }
        _inertiaRotationOffset = Quaternion.identity;
        _inertiaPositionOffset = Vector3.zero;
        _lookReturnToPositionCoroutine = null;
    }
    private void UpdateLook()
    {
        float recoilInertiaReduction = _isAiming ? m_weaponMovementData.m_lookReductionWhileAiming : 1;

        Vector2 lookVelocity = GameManager.GetInstance().m_playerController.m_characterLook.GetLookVelocity();
        lookVelocity.y += GameManager.GetInstance().m_playerController.m_characterController.velocity.y;
        if (lookVelocity.magnitude <= 0.1f)
        {
            if (_lookReturnToPositionCoroutine == null && (_inertiaRotationOffset != Quaternion.identity || _inertiaPositionOffset != Vector3.zero))
            {
                ReturnToPositionInertiaAnimation();
            }
        }
        else
        {
            Vector2 rawLookVelocity = GameManager.GetInstance().m_playerController.m_characterLook.GetLookVelocity();
            _lookVelocitySmooth = Vector2.SmoothDamp(_lookVelocitySmooth, rawLookVelocity, ref _lookVelocityRef, 0.1f); // 0.01s smooth
            lookVelocity = _lookVelocitySmooth;

            if (_lookReturnToPositionCoroutine != null)
            {
                StopCoroutine(_lookReturnToPositionCoroutine);
                _lookReturnToPositionCoroutine = null;
            }

            Quaternion targetRot = Quaternion.Euler(
                Mathf.Clamp(-lookVelocity.y * m_weaponMovementData.m_lookAngleRotationSpeed * recoilInertiaReduction, -m_weaponMovementData.m_lookMaxAngleRotation.y, m_weaponMovementData.m_lookMaxAngleRotation.y),
                Mathf.Clamp(lookVelocity.x * m_weaponMovementData.m_lookAngleRotationSpeed * recoilInertiaReduction, -m_weaponMovementData.m_lookMaxAngleRotation.x, m_weaponMovementData.m_lookMaxAngleRotation.x),
                0f);

            Vector3 targetPos = new Vector3(
                Mathf.Clamp(-lookVelocity.x * m_weaponMovementData.m_lookHorizontalOffsetSpeed * recoilInertiaReduction, -m_weaponMovementData.m_lookMaxHorizontalOffset, m_weaponMovementData.m_lookMaxHorizontalOffset),
                0f,
                0f);

            if (_isAiming)
            {
                targetPos = Vector3.zero;
            }

            _inertiaPositionOffset = Vector3.Slerp(_inertiaPositionOffset, targetPos, Time.deltaTime * m_weaponMovementData.m_lookHorizontalOffsetSpeed);
            _inertiaRotationOffset = Quaternion.Slerp(_inertiaRotationOffset, targetRot, Time.deltaTime * m_weaponMovementData.m_lookAngleRotationSpeed);
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
        Vector3 endPosition = starting ? m_weaponMovementData.m_walkingLocalPosition - m_weaponMovementData.m_idleLocalPosition : Vector3.zero;

        Quaternion startRotation = starting ? Quaternion.identity : Quaternion.Euler(m_weaponMovementData.m_walkingLocalRotation);
        Quaternion endRotation = starting ? Quaternion.Euler(m_weaponMovementData.m_walkingLocalRotation) : Quaternion.identity;
        while (currentTime <= m_weaponMovementData.m_walkingPositionAndRotationTime)
        {
            float valueCurve = m_weaponMovementData.m_walkingPositionAndRotationCurve.Evaluate((currentTime / m_weaponMovementData.m_walkingPositionAndRotationTime) % 1);

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
            float recoilInertiaReduction = _isAiming ? m_weaponMovementData.m_lookReductionWhileAiming : 1;

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
                _currentVerticalBounceTime += Time.deltaTime / m_weaponMovementData.m_walkingBounceVerticalTime;
                _currentHorizontalBounceTime += Time.deltaTime / m_weaponMovementData.m_walkingBounceHorizontalTime;
                _currentForwardRotationTime += Time.deltaTime / m_weaponMovementData.m_walkingRotationLoopForwardTime;

                Vector3 newPosition = new Vector3(
                    m_weaponMovementData.m_walkingBounceHorizontalCurve.Evaluate(_currentHorizontalBounceTime % 1) * m_weaponMovementData.m_walkingBounceHorizontalAmplitude * recoilInertiaReduction,
                    m_weaponMovementData.m_walkingBounceVerticalCurve.Evaluate(_currentVerticalBounceTime % 1) * m_weaponMovementData.m_walkingBounceVerticalAmplitude * recoilInertiaReduction,
                    0.0f
                    );

                Quaternion newRotation = Quaternion.Euler(
                    0.0f,
                    0.0f,
                    m_weaponMovementData.m_walkingRotationLoopForwardCurve.Evaluate(_currentForwardRotationTime % 1) * m_weaponMovementData.m_walkingRotationForwardAngle * recoilInertiaReduction
                    );


                //Dir translation
                Vector3 charDirection = GameManager.GetInstance().m_inputManager.m_move.ReadValue<Vector2>();
                charDirection.x *= m_weaponMovementData.m_walkingOffsetHorizontalDistance * recoilInertiaReduction;
                charDirection.z = -charDirection.y * m_weaponMovementData.m_walkingOffsetForwardDistance * recoilInertiaReduction;
                charDirection.y = 0;

                _currentDirectionTranslation += charDirection * m_weaponMovementData.m_walkingOffsetPositionSpeed * Time.deltaTime;

                _currentDirectionTranslation.x = Mathf.Clamp(_currentDirectionTranslation.x, -m_weaponMovementData.m_walkingOffsetHorizontalDistance, m_weaponMovementData.m_walkingOffsetHorizontalDistance);
                _currentDirectionTranslation.z = Mathf.Clamp(_currentDirectionTranslation.z, -m_weaponMovementData.m_walkingOffsetForwardDistance, m_weaponMovementData.m_walkingOffsetForwardDistance);

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

    public void Fire()
    {
        if (_coroutineFire != null)
        {
            StopCoroutine(_coroutineFire);
        }
        _coroutineFire = StartCoroutine(FireAnimation());
    }

    IEnumerator FireAnimation()
    {
        float recoilInertiaReduction = _isAiming ? m_weaponMovementData.m_lookReductionWhileAiming : 1;
        float recoilReductionWhileAiming = _isAiming ? m_weaponMovementData.m_recoilReductionWhileAiming : 1;
        //_isRecoiling = true;
        float _currentRecoilSpeedTime = 0;
        float _currentRecoilRecoverySpeedTime = 0;

        Vector3 startPosition = _recoilPositionOffset;
        Vector3 endPosition = _recoilPositionOffset + m_weaponMovementData.m_recoilAdditivePosition * recoilInertiaReduction;
        endPosition.x = Mathf.Clamp(endPosition.x, -m_weaponMovementData.m_recoilMaxPosition.x * recoilInertiaReduction, m_weaponMovementData.m_recoilMaxPosition.x * recoilInertiaReduction);
        endPosition.y = Mathf.Clamp(endPosition.y, -m_weaponMovementData.m_recoilMaxPosition.y * recoilInertiaReduction, m_weaponMovementData.m_recoilMaxPosition.y * recoilInertiaReduction);
        endPosition.z = Mathf.Clamp(endPosition.z, -m_weaponMovementData.m_recoilMaxPosition.z * recoilInertiaReduction, m_weaponMovementData.m_recoilMaxPosition.z * recoilInertiaReduction);

        Vector3 randomRotation = new Vector3(UnityEngine.Random.Range(-1, 1.0f), UnityEngine.Random.Range(-1, 1.0f), 0) * UnityEngine.Random.Range(m_weaponMovementData.m_recoilRandomRotationY.x, m_weaponMovementData.m_recoilRandomRotationY.y);
        Quaternion startRotation = _recoilRotationOffset;
        Vector3 endRotationClamped = (_recoilRotationOffset.eulerAngles + (m_weaponMovementData.m_recoilAdditiveRotation + randomRotation) * recoilReductionWhileAiming);

        endRotationClamped.x = Mathf.Clamp(endRotationClamped.x > 180 ? endRotationClamped.x - 360 : endRotationClamped.x, -m_weaponMovementData.m_recoilMaxRotation.x * recoilInertiaReduction, m_weaponMovementData.m_recoilMaxRotation.x * recoilInertiaReduction);
        endRotationClamped.y = Mathf.Clamp(endRotationClamped.y > 180 ? endRotationClamped.y - 360 : endRotationClamped.y, -m_weaponMovementData.m_recoilMaxRotation.y * recoilInertiaReduction, m_weaponMovementData.m_recoilMaxRotation.y * recoilInertiaReduction);
        endRotationClamped.z = Mathf.Clamp(endRotationClamped.z > 180 ? endRotationClamped.z - 360 : endRotationClamped.z, -m_weaponMovementData.m_recoilMaxRotation.z * recoilInertiaReduction, m_weaponMovementData.m_recoilMaxRotation.z * recoilInertiaReduction);
        Quaternion endRotation = Quaternion.Euler(endRotationClamped);

        while (_currentRecoilSpeedTime <= m_weaponMovementData.m_recoilStartTime)
        {
            float curveRotationT = m_weaponMovementData.m_recoilRotationCurve.Evaluate(_currentRecoilSpeedTime / m_weaponMovementData.m_recoilStartTime);
            float curvePositionT = m_weaponMovementData.m_recoilPositionCurve.Evaluate(_currentRecoilSpeedTime / m_weaponMovementData.m_recoilStartTime);

            _recoilPositionOffset = Vector3.Slerp(startPosition, endPosition, curvePositionT);
            _recoilRotationOffset = Quaternion.Slerp(startRotation, endRotation, curveRotationT);

            _currentRecoilSpeedTime += Time.deltaTime;
            yield return null;
        }

        while (_currentRecoilRecoverySpeedTime <= m_weaponMovementData.m_recoilRecoverySpeedTime)
        {
            float curveT = m_weaponMovementData.m_recoveryRecoilCurve.Evaluate(_currentRecoilRecoverySpeedTime / m_weaponMovementData.m_recoilRecoverySpeedTime);
            _recoilPositionOffset = Vector3.Slerp(endPosition, Vector3.zero, curveT);
            _recoilRotationOffset = Quaternion.Slerp(endRotation, Quaternion.identity, curveT);

            _currentRecoilRecoverySpeedTime += Time.deltaTime;
            yield return null;

        }

        _recoilPositionOffset = Vector3.zero;
        _recoilRotationOffset = Quaternion.identity;
        //_isRecoiling = false;
        _coroutineFire = null;
    }

    public void CanAim(bool canAim)
    {
        _canAim = canAim;
    }

    public void StartAim()
    {
        if (_isAiming || !_canAim)
        {
            return;
        }
        if (_coroutineAim != null)
        {
            StopCoroutine(_coroutineAim);
        }
        _coroutineAim = StartCoroutine(AimAnimation(true));
    }

    private void StartAim(InputAction.CallbackContext context)
    {
        StartAim();
    }

    public void EndAim()
    {
        if (!_isAiming)
        {
            return;
        }
        if (_coroutineAim != null)
        {
            StopCoroutine(_coroutineAim);
        }
        _coroutineAim = StartCoroutine(AimAnimation(false));
    }

    private void EndAim(InputAction.CallbackContext context)
    {
        EndAim();
    }

    IEnumerator AimAnimation(bool isStarting)
    {
        _isAiming = isStarting;
        GameManager.GetInstance().m_playerController.GetCurrentWeapon().Aiming(isStarting);
        float _currentTime = 0;
        Vector3 startPosition = _aimPositionOffset;
        Quaternion startRotation = _aimRotationOffset;

        Vector3 endPosition = isStarting ? m_weaponMovementData.m_aimPosition - _currentSocketPosition : Vector3.zero;
        Quaternion endRotation = isStarting ? Quaternion.Euler(m_weaponMovementData.m_aimRotation - _currentSocketRotation.eulerAngles) : Quaternion.identity;

        //temp
        GameManager.GetInstance().m_crosshairController.ShowOrHideCrosshair(!isStarting);

        while (_currentTime <= m_weaponMovementData.m_aimStartTime)
        {
            _aimPositionOffset = Vector3.Slerp(startPosition, endPosition, _currentTime / m_weaponMovementData.m_aimStartTime);
            _aimRotationOffset = Quaternion.Slerp(startRotation, endRotation, _currentTime / m_weaponMovementData.m_aimStartTime);
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
        endPosition = endPosition.normalized * m_weaponMovementData.m_dampingAmplitude;

        while (_currentTime <= m_weaponMovementData.m_dampingTime)
        {
            float valueT = m_weaponMovementData.m_dampingCurve.Evaluate(_currentTime / m_weaponMovementData.m_dampingTime);
            _dampingPositionOffset = Vector3.Slerp(startPosition, endPosition, valueT);
            _currentTime += Time.deltaTime;
            yield return null;
        }
        _dampingPositionOffset = Vector3.zero;
        _coroutineDamping = null;
    }
}
