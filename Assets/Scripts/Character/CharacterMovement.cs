using System;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class CharacterMovement : MonoBehaviour
{
    //Public
    [Header("References")]
    public GameObject m_head;

    [Header("Values")]
    public float m_gravity = -9.8f;
    public float m_characterWalkngSpeed = 6.0f;
    public float m_characterRunningSpeed = 9.0f;
    public float m_characterDownedSpeed = 1.0f;
    public float m_jumpHeight = 1.5f;
    public float m_standHeight = 2f;
    public float m_crouchHeight = 1f;
    public float m_downedHeight = 0.5f;
    public float m_stamina = 3f;

    //Private
    private float _currentPlayerSpeed;
    private Vector3 _velocity;
    private CharacterController _characterController;
    private PlayerController _playerController;
    private float m_currentStamina;
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        // If the Character Controller component doesn't exist, add one
        if (_characterController == null)
        {
            _characterController = gameObject.AddComponent<CharacterController>();
        }

        _currentPlayerSpeed = m_characterWalkngSpeed;
        _playerController = GetComponent<PlayerController>();
        m_currentStamina = GetStamina();
        InitializeInputBinding();
    }

    public void EvaluateMoving()
    {
        //Evaluate moving temp
        Vector3 moveValue = transform.right * GetMovementInput().x + transform.forward * GetMovementInput().z;
        moveValue.x = moveValue.x * _currentPlayerSpeed;
        moveValue.z = moveValue.z * _currentPlayerSpeed;
        if (moveValue.magnitude < 1)
        {
            _playerController.SetIdleValues();
            
        }
        else
        {
            _playerController.SetMovingValues();
            if (_currentPlayerSpeed.Equals(m_characterRunningSpeed))
            {
                m_currentStamina -= Time.deltaTime;
            }
        }

        if (_currentPlayerSpeed.Equals(m_characterWalkngSpeed))
        {
            m_currentStamina += Time.deltaTime;
        }
        m_currentStamina = Mathf.Clamp(m_currentStamina, 0, GetStamina());

        if (m_currentStamina <= 0)
        {
            SetCurrentPlayerSpeed(m_characterWalkngSpeed);
        }
    }

    private void OnDestroy()
    {
        DeinitilizeInputBinding();
    }



    void InitializeInputBinding()
    {
        GameManager.GetInstance().m_inputManager.m_sprint.started += StartRunning;
        GameManager.GetInstance().m_inputManager.m_sprint.canceled += EndRunning;

        GameManager.GetInstance().m_inputManager.m_jump.started += StartJumping;

        GameManager.GetInstance().m_inputManager.m_crouch.started += EvaluateCrouch;
    }

    void DeinitilizeInputBinding()
    {
        GameManager.GetInstance().m_inputManager.m_sprint.started -= StartRunning;
        GameManager.GetInstance().m_inputManager.m_sprint.canceled -= EndRunning;

        GameManager.GetInstance().m_inputManager.m_jump.started -= StartJumping;

        GameManager.GetInstance().m_inputManager.m_crouch.started -= EvaluateCrouch;

    }

    public void UpdateMove()
    {
        Vector3 moveValue = transform.right * GetMovementInput().x + transform.forward * GetMovementInput().z;
        _velocity.x = moveValue.x * _currentPlayerSpeed;
        _velocity.z = moveValue.z * _currentPlayerSpeed;

        MoveCharacter();
    }

    public void UpdateAirMove()
    {
        //Temp - Move Character - TODO : real velocity
        Vector3 moveValue = transform.right * GetMovementInput().x + transform.forward * GetMovementInput().z;
        _velocity.x = moveValue.x * _currentPlayerSpeed / 2;
        _velocity.z = moveValue.z * _currentPlayerSpeed / 2;

        MoveCharacter();
    }

    private void MoveCharacter()
    {
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            //Resets the gravity
            //A small value is needed so the Character Controller detects the ground every frame
            _velocity.y = -1f;
        }

        _velocity.y += m_gravity * Time.deltaTime;

        _characterController.Move(_velocity * Time.deltaTime);
    }

    public void SetCurrentPlayerSpeed(float speed)
    {
        _currentPlayerSpeed = speed;
    }

    public float GetCurrentPlayerSpeed()
    {
        return _currentPlayerSpeed;
    }

    public void Jump()
    {
        _velocity.y = Mathf.Sqrt(m_jumpHeight * -2f * m_gravity);
    }

    public void SetCurrentHeight(float height)
    {
        _characterController.height = height;
    }
    public void SetCurrentRadius(float radius)
    {
        _characterController.radius = radius;
    }

    Vector3 GetMovementInput()
    {
        Vector2 value = GameManager.GetInstance().m_inputManager.m_move.ReadValue<Vector2>();
        return new Vector3(value.x, 0, value.y);
    }

    void StartRunning(InputAction.CallbackContext context)
    {
        if (_playerController.m_characterHealth.GetCurrentHealth() <= 0)
        {
            return;
        }
        SetCurrentPlayerSpeed(m_characterRunningSpeed);
        //_playerController.GetStateMachine().SetCurrentState<Running>();
    }

    void EndRunning(InputAction.CallbackContext context)
    {
        if (_playerController.m_characterHealth.GetCurrentHealth() <= 0)
        {
            return;
        }
        SetCurrentPlayerSpeed(m_characterWalkngSpeed);
        //_playerController.GetStateMachine().SetCurrentState<Idle>();
    }

    void StartJumping(InputAction.CallbackContext context)
    {
        if (_characterController.isGrounded)
        {
            _playerController.GetStateMachine().SetCurrentState<Jumping>();
        }
    }

    void EvaluateCrouch(InputAction.CallbackContext context)
    {
        if (!_playerController.GetStateMachine().CurrentStateIs<Crouching>())
        {
            _playerController.GetStateMachine().SetCurrentState<Crouching>();
        }
        else
        {
            _playerController.GetStateMachine().SetCurrentState<IdleOrMoving>();
        }
    }

    private float GetStamina()
    {
        return m_stamina + GameManager.GetInstance().m_playerController.m_characterPerks.m_perksData.m_extraStamina;
    }
}
