using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController m_characterController;
    public CharacterMovement m_characterMovement;
    public CharacterLook m_characterLook;

    private StateMachineFilter _stateMachine;

    private void Awake()
    {
        InitializeStateMachine();
    }

    void InitializeStateMachine()
    {
        _stateMachine = new StateMachineFilter();
        _stateMachine.InitializeStateMachine<Idle>(GameManager.GetInstance().m_characterStatesData.GetStatesData());
    }

    public StateMachineFilter GetStateMachine()
    {
        return _stateMachine;
    }
}
