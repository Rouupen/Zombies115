using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages all input actions using Unity's Input System. Initializes actions based on mappings stored in SO_InputData
/// </summary>
public class InputManager : Manager
{
    [HideInInspector]
    public List<InputAction> m_actions = new List<InputAction>();
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

    /// <summary>
    /// Initializes the input actions by retrieving them from the PlayerInput component
    /// and mapping them based on the input data defined in SO_InputData
    /// </summary>
    public override void Initialize()
    {
        _playerInput = GameManager.GetInstance().m_playerInput;
        
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

        // I don't know if there is a better way to map it
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

    /// <summary>
    /// Cleans up the InputManager
    /// </summary>
    public override void Deinitialize()
    {

    }
}
