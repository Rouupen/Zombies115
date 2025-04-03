using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AllowedStatesFilter
{
    public System.Type m_mainState;
    public List<System.Type> m_allowedStates;
}

public abstract class State
{
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}

public class StateMachineFilter
{
    private State _currentState;
    private List<AllowedStatesFilter> _statesData;
    private bool IsStateMachineInitialized()
    {
        if (_currentState != null)
        {
            return true;
        }
        return false;
    }

    // Initialize the StateMachine with the Type T (need a empty constructor)
    public void InitializeStateMachine<T>(List<AllowedStatesFilter> statesData) where T : State, new()
    {
        if (IsStateMachineInitialized())
        {
            Debug.LogError("The state machine is already initialized. State: " + _currentState.ToString());
            return;
        }
        _currentState = new T();
        _currentState.OnEnter();
        _statesData = statesData;
        GameManager.GetInstance().m_updateStateMachines += UpdateStateMachine;
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
        if (!IsStateMachineInitialized())
        {
            return false;
        }
        
        if (!IsStateAvailable<T>())
        {
            return false;
        }

        _currentState.OnExit();
        _currentState = new T();
        _currentState.OnEnter();

        return true;
    }


    public bool IsStateAvailable<T>() where T : State, new()
    {
        if (_statesData == null || _statesData.Count == 0 || _currentState.GetType() == typeof(T))
        {
            return false;
        }

        for (int i = 0; i < _statesData.Count; i++)
        {
            if (_statesData[i].m_mainState == _currentState.GetType())
            {
                for (int j = 0; j < _statesData[i].m_allowedStates.Count; j++)
                {
                    if (_statesData[i].m_allowedStates[j] == typeof(T))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        return false;
    }

    public void UpdateStateMachine()
    {
        if (IsStateMachineInitialized())
        {
            _currentState.OnUpdate();
        }
    }

}
