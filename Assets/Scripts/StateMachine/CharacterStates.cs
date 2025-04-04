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

public class Crouching : State
{
    public override void OnEnter()
    {
        Debug.Log("OnEnter Crouching");
    }

    public override void OnUpdate()
    {
        Debug.Log("OnUpdate Crouching");
    }

    public override void OnExit()
    {
        Debug.Log("OnExit Crouching");
    }
}

public class Crawling : State
{
    public override void OnEnter()
    {
        Debug.Log("OnEnter Crawling");
    }

    public override void OnUpdate()
    {
        Debug.Log("OnUpdate Crawling");
    }

    public override void OnExit()
    {
        Debug.Log("OnExit Crawling");
    }
}

public class Jumping : State
{
    public override void OnEnter()
    {
        Debug.Log("OnEnter Jumping");
    }

    public override void OnUpdate()
    {
        Debug.Log("OnUpdate Jumping");
    }

    public override void OnExit()
    {
        Debug.Log("OnExit Jumping");
    }
}

public class OnAir : State
{
    public override void OnEnter()
    {
        Debug.Log("OnEnter OnAir");
    }

    public override void OnUpdate()
    {
        Debug.Log("OnUpdate OnAir");
    }

    public override void OnExit()
    {
        Debug.Log("OnExit OnAir");
    }
}

public class Interacting : State
{
    public override void OnEnter()
    {
        Debug.Log("OnEnter Interacting");
    }

    public override void OnUpdate()
    {
        Debug.Log("OnUpdate Interacting");
    }

    public override void OnExit()
    {
        Debug.Log("OnExit Interacting");
    }
}

