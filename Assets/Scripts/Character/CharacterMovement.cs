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
    public float m_characterSpeed = 10.0f;
    public float m_jumpHeight = 1.5f;
    
    //Private
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
        _stateMachine = new StateMachineFilter();
        _stateMachine.InitializeStateMachine<CharacterStates.StateIdle>(GameManager.GetInstance().m_characterStatesData.GetStatesDictionary());
        _stateMachine.SetCurrentState<CharacterStates.StateIdle>();
    }

    void Update()
    {
        //Temp - Move Character
        Vector3 moveValue = transform.right * GetMovementInput().x + transform.forward * GetMovementInput().z;
        _velocity.x = moveValue.x * m_characterSpeed;
        _velocity.z = moveValue.z * m_characterSpeed;

        //Temp - Sprint
        if (IsSprinting())
        {
            _velocity.x *= 1.5f;
            _velocity.z *= 1.5f;
        }

        //Temp - Jump
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            //Resets the gravity
            //A small value is needed so the Character Controller detects the ground every frame
            _velocity.y = -1f;
        }

        if (_characterController.isGrounded && IsTryingToJump())
        {
            _velocity.y = Mathf.Sqrt(m_jumpHeight * -2f * m_gravity);
        }
        _velocity.y += m_gravity * Time.deltaTime;


        _characterController.Move(_velocity * Time.deltaTime);

    }

    Vector3 GetMovementInput()
    {
        //Temp - Read "Move" input value
        Vector2 value = GameManager.GetInstance().m_inputManager.m_move.ReadValue<Vector2>();
        return new Vector3(value.x, 0, value.y);
    }

    bool IsTryingToJump()
    {
        //Temp - Read if "Jump" is pressed
        return GameManager.GetInstance().m_inputManager.m_jump.IsPressed();
    }

    bool IsSprinting()
    {
        //Temp - Read if "Sprint" is pressed
        return GameManager.GetInstance().m_inputManager.m_sprint.IsPressed();
    }

}
