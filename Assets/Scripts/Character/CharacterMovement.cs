using System;
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
    public float m_jumpHeight = 1.5f;
    public float m_standHeight = 2f;
    public float m_crouchHeight = 1f;

    //Private
    private float _currentPlayerSpeed;
    private Vector3 _velocity;
    private CharacterController _characterController;
    private StateMachineFilter _stateMachine;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        // If the Character Controller component doesn't exist, add one
        if (_characterController == null)
        {
            _characterController = gameObject.AddComponent<CharacterController>();
        }

        _currentPlayerSpeed = m_characterWalkngSpeed;

        InitializeStateMachine();

        InitializeInputBinding();

    }

    private void OnDestroy()
    {
        DeinitilizeInputBinding();
    }

    void InitializeStateMachine()
    {
        _stateMachine = new StateMachineFilter();
        _stateMachine.InitializeStateMachine<Idle>(GameManager.GetInstance().m_characterStatesData.GetStatesData());
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

    Vector3 GetMovementInput()
    {
        Vector2 value = GameManager.GetInstance().m_inputManager.m_move.ReadValue<Vector2>();
        return new Vector3(value.x, 0, value.y);
    }

    void StartRunning(InputAction.CallbackContext context)
    {
        _stateMachine.SetCurrentState<Running>();
    }

    void EndRunning(InputAction.CallbackContext context)
    {
        _stateMachine.SetCurrentState<Idle>();
    }

    void StartJumping(InputAction.CallbackContext context)
    {
        if (_characterController.isGrounded)
        {
            _stateMachine.SetCurrentState<Jumping>();
        }
    }

    void EvaluateCrouch(InputAction.CallbackContext context)
    {
        if (!_stateMachine.CurrentStateIs<Crouching>())
        {
            _stateMachine.SetCurrentState<Crouching>();
        }
        else
        {
            _stateMachine.SetCurrentState<Idle>();
        }
    }

    public StateMachineFilter GetStateMachine()
    {
        return _stateMachine;
    }
}
