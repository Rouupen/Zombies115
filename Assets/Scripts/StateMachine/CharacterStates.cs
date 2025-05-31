using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CharacterStates
{
    IdleOrMoving,
    Crouching,
    Crawling,
    Jumping,
    OnAir,
    Interacting,
    Downed
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

public class IdleOrMoving : CharacterState
{
    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.SetIdleValues();
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
    }
}

//public class Walking : CharacterState
//{
//    public override void OnEnter()
//    {
//        base.OnEnter();
//        _playerController.SetMovingValues();
//    }

//    public override void OnUpdate()
//    {
//        base.OnUpdate();
//        _playerController.m_characterMovement.UpdateMove();
//        _playerController.m_characterMovement.EvaluateMoving();
//    }

//    public override void OnExit()
//    {
//        base.OnExit();
//    }
//}

//public class Running : CharacterState
//{
//    public override void OnEnter()
//    {
//        base.OnEnter();
//        _playerController.m_characterMovement.SetCurrentPlayerSpeed(_playerController.m_characterMovement.m_characterRunningSpeed);
//        _playerController.SetMovingValues();

//    }

//    public override void OnUpdate()
//    {
//        base.OnUpdate();
//        _playerController.m_characterMovement.UpdateMove();
//        _playerController.m_characterMovement.EvaluateMoving();

//    }

//    public override void OnExit()
//    {
//        base.OnExit();
//        _playerController.m_characterMovement.SetCurrentPlayerSpeed(_playerController.m_characterMovement.m_characterWalkngSpeed);
//    }
//}

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
            _playerController.GetStateMachine().SetCurrentState<IdleOrMoving>();
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

public class Downed : CharacterState
{
    Coroutine _anim;
    bool _revive;
    public override void OnEnter()
    {
        base.OnEnter();
        if (_playerController.m_characterPerks.m_perksData.m_revive)
        {
            _anim = GameManager.GetInstance().StartCoroutine(ReviveAnim());
            _revive = true;
        }
        else
        {
            _anim = GameManager.GetInstance().StartCoroutine(EndGameAnim());
            _revive = false;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //if (_revive)
        //{
            _playerController.m_characterMovement.UpdateMove();
            _playerController.m_characterMovement.m_stamina = 0;
        //}
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    IEnumerator EndGameAnim()
    {
        float time = 1;
        float currentTime = 0;
        _playerController.m_characterMovement.SetCurrentRadius(0.2f);
        _playerController.m_characterMovement.SetCurrentHeight(_playerController.m_characterMovement.m_downedHeight);
        _playerController.m_characterPerks.RemoveAllPerks();
        _playerController.m_characterMovement.SetCurrentPlayerSpeed(0);
        _playerController.GetCurrentWeapon().HideWeapon();
        while (currentTime <= time)
        {
            _playerController.m_volumeDeath.weight = Mathf.Lerp(0, 1, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        time = 5;

        while (currentTime <= time)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        _anim = null;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    //TEMP
    IEnumerator ReviveAnim()
    {
        float time = 0.5f;
        float currentTime = 0;
        _playerController.m_characterMovement.SetCurrentRadius(0.2f);
        _playerController.m_characterMovement.SetCurrentHeight(_playerController.m_characterMovement.m_downedHeight);
        _playerController.m_characterMovement.SetCurrentPlayerSpeed(_playerController.m_characterMovement.m_characterDownedSpeed);
        _playerController.m_characterPerks.RemoveAllPerks();
        GameManager.GetInstance().m_spawnManager.EnemysRunAwayTarget();
        while (currentTime <= time)
        {
            _playerController.m_volumeDeath.weight = Mathf.Lerp(0, 1, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }

        time = 10;
        currentTime = 0;
        while (currentTime <= time)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        _playerController.m_characterHealth.InitializeEntityHealth(100);
        _playerController.m_characterHealth.TakeDamage(0);

        _playerController.m_characterMovement.SetCurrentHeight(_playerController.m_characterMovement.m_standHeight);

        time = 1.5f;
        currentTime = 0;
        _playerController.m_characterMovement.SetCurrentRadius(0.5f);

        _playerController.m_characterMovement.SetCurrentPlayerSpeed(_playerController.m_characterMovement.m_characterWalkngSpeed);
        while (currentTime <= time)
        {
            _playerController.m_volumeDeath.weight = 1 - Mathf.Lerp(0, 1, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        GameManager.GetInstance().m_spawnManager.SetPlayerAsTargetForEnemys();

        _anim = null;

        _playerController.GetStateMachine().SetCurrentState<IdleOrMoving>();

    }
}

