using UnityEngine;

public enum CharacterStates
{
    Idle,
    Running
}


public class Idle : State
{
    public override void OnEnter()
    {
        Debug.Log("OnEnter Idle");
    }

    public override void OnUpdate()
    {
        Debug.Log("OnUpdate Idle");
    }

    public override void OnExit()
    {
        Debug.Log("OnExit Idle");
    }
}

public class Running : State
{
    public override void OnEnter()
    {
        Debug.Log("OnEnter Running");
    }

    public override void OnUpdate()
    {
        Debug.Log("OnUpdate Running");
    }

    public override void OnExit()
    {
        Debug.Log("OnExit Running");
    }
}

