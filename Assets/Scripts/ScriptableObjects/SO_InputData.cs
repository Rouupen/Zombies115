using UnityEngine;
using UnityEngine.InputSystem;

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
}
