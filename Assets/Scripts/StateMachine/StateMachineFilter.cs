using System.Collections.Generic;
using UnityEngine;

public enum StateName
{
}

public abstract class State
{
    public StateName m_stateName;
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}

public class StateMachineFilter
{
    private State _currentState;
    private Dictionary<State, System.Type> _dictionary;
    private bool IsStateMachineInitialized()
    {
        if (_currentState != null)
        {
            return true;
        }
        return false;
    }

    // Initialize the StateMachine with the Type T (need a empty constructor)
    public void InitializeStateMachine<T>(Dictionary<State, System.Type> dictionary) where T : State, new()
    {
        if (IsStateMachineInitialized())
        {
            Debug.LogError("The state machine is already initialized. State: " + _currentState.ToString());
            return;
        }
        _currentState = new T();
        _currentState.OnEnter();
        _dictionary = dictionary;
    }

    public void DeinitializeStateMachine()
    {
        if (IsStateMachineInitialized())
        {
            _currentState.OnExit();
            _currentState = null;
        }
    }

    public bool SetCurrentState<T>() where T : State, new()
    {
        if (IsStateMachineInitialized())
        {
            return false;
        }

        if (!IsStateAvailable()) 
        {
            return false;
        }
        //TEMP
        _currentState.OnExit();
        _currentState = new T(); 
        _currentState.OnEnter();
        return false;
    }


    public bool IsStateAvailable()
    {
        return true;
    }

    public void UpdateStateMachine()
    {
        if (IsStateMachineInitialized()) 
        {
            return;
        }
        _currentState.OnUpdate();
    }

}
