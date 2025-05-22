using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Contains configurable input action names used to bind PlayerInput actions dynamically
/// </summary>
[CreateAssetMenu(fileName = "InputData", menuName = "ScriptableObjects/InputData", order = 1)]
public class SO_InputData : ScriptableObject
{
    public string m_move;
    public string m_look;
    public string m_fire;
    public string m_aim;
    public string m_interact;
    public string m_crouch;
    public string m_jump;
    public string m_sprint;
    public string m_weaponSelection1;
    public string m_weaponSelection2;
    public string m_reload;
    public string m_mouseWheel;
}
