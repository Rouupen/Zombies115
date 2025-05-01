using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

public class InputManager : Manager
{
    private PlayerInput _playerInput;

    public InputAction m_move;
    public InputAction m_look;
    public InputAction m_fire;
    public InputAction m_aim;
    public InputAction m_interact;
    public InputAction m_crouch;
    public InputAction m_jump;
    public InputAction m_sprint;
    public InputAction m_weaponSelection1;
    public InputAction m_weaponSelection2;
    public InputAction m_reload;

    [HideInInspector]
    public List<InputAction> m_actions = new List<InputAction>();

    public override void Initialize()
    {
        _playerInput = GameManager.GetInstance().GetComponent<PlayerInput>();
        
        if (_playerInput == null)
        {
            Debug.LogError("There is no player input in the GameManager");
            return;
        }
        
        SO_InputData inputData = GameManager.GetInstance().m_inputData;
        if (inputData == null)
        {
            Debug.LogError("There is no input data in the GameManager");
            return;
        }

        m_move = _playerInput.actions.FindAction(inputData.m_move);
        m_look = _playerInput.actions.FindAction(inputData.m_look);
        m_fire = _playerInput.actions.FindAction(inputData.m_fire);
        m_aim = _playerInput.actions.FindAction(inputData.m_aim);
        m_interact = _playerInput.actions.FindAction(inputData.m_interact);
        m_crouch = _playerInput.actions.FindAction(inputData.m_crouch);
        m_jump = _playerInput.actions.FindAction(inputData.m_jump);
        m_sprint = _playerInput.actions.FindAction(inputData.m_sprint);
        m_weaponSelection1 = _playerInput.actions.FindAction(inputData.m_weaponSelection1);
        m_weaponSelection2 = _playerInput.actions.FindAction(inputData.m_weaponSelection2);
        m_reload = _playerInput.actions.FindAction(inputData.m_reload);
    }

    public override void Deinitialize()
    {

    }
}
