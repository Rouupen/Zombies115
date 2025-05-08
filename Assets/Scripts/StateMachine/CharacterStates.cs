using UnityEngine;

public enum CharacterStates
{
    Idle,
    Walking,
    Running,
    Crouching,
    Crawling,
    Jumping,
    OnAir,
    Interacting
}
/// <summary>
/// All character states
/// </summary>
public class CharacterState : State
{
    protected PlayerController _playerController;
    
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController = _gameManagerInstance.m_playerController;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

public class Idle : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.SetIdleValues();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _playerController.m_characterMovement.EvaluateMoving();

    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

public class Walking : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.SetMovingValues();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _playerController.m_characterMovement.UpdateMove();
        _playerController.m_characterMovement.EvaluateMoving();
    }

    public override void OnExit()
    {
        base.OnExit();
        _playerController.m_characterMovement.SetCurrentPlayerSpeed(_playerController.m_characterMovement.m_characterWalkngSpeed);
    }
}

public class Running : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.m_characterMovement.SetCurrentPlayerSpeed(_playerController.m_characterMovement.m_characterRunningSpeed);
        _playerController.SetMovingValues();

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _playerController.m_characterMovement.UpdateMove();
        _playerController.m_characterMovement.EvaluateMoving();

    }

    public override void OnExit()
    {
        base.OnExit();
        _playerController.m_characterMovement.SetCurrentPlayerSpeed(_playerController.m_characterMovement.m_characterWalkngSpeed);
    }
}

public class Crouching : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.m_characterMovement.SetCurrentHeight(_playerController.m_characterMovement.m_crouchHeight);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _playerController.m_characterMovement.UpdateMove();

    }

    public override void OnExit()
    {
        _playerController.m_characterMovement.SetCurrentHeight(_playerController.m_characterMovement.m_standHeight);
        base.OnExit();
    }
}

public class Crawling : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

public class Jumping : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.m_characterMovement.Jump();
        _playerController.SetMovingValues();
        _playerController.GetStateMachine().SetCurrentState<OnAir>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

public class OnAir : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.SetMovingValues();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _playerController.m_characterMovement.UpdateAirMove();

        if (_playerController.m_characterController.isGrounded)
        {
            _playerController.GetStateMachine().SetCurrentState<Idle>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

public class Interacting : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

