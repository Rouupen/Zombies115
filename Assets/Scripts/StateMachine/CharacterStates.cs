using UnityEngine;

public enum CharacterStates
{
    Idle,
    Running,
    Crouching,
    Crawling,
    Jumping,
    OnAir,
    Interacting
}

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
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _playerController.m_characterMovement.UpdateMove();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

public class Running : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.m_characterMovement.SetCurrentPlayerSpeed(_playerController.m_characterMovement.m_characterRunningSpeed);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _playerController.m_characterMovement.UpdateMove();
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
        _playerController.m_characterMovement.GetStateMachine().SetCurrentState<OnAir>();
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
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _playerController.m_characterMovement.UpdateAirMove();

        if (_playerController.m_characterController.isGrounded)
        {
            _playerController.m_characterMovement.GetStateMachine().SetCurrentState<Idle>();
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

