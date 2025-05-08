using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines allowed transitions between states
/// </summary>
public struct AllowedStatesFilter
{
    public System.Type m_mainState;
    public List<System.Type> m_allowedStates;
}

/// <summary>
/// Base class for all states used in the state machine
/// </summary>
public abstract class State
{
    protected GameManager _gameManagerInstance;

    public virtual void OnEnter() 
    {
        _gameManagerInstance = GameManager.GetInstance();
    }
    public virtual void OnUpdate()
    { 
    
    }
    public virtual void OnExit()
    {

    }
}

/// <summary>
/// Handles state transitions and updates for a given set of allowed states
/// </summary>
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
            GameManager.GetInstance().m_updateStateMachines -= UpdateStateMachine;
        }
    }

    /// <summary>
    /// Attempts to switch to a new state, if the transition is allowed
    /// </summary>
    /// /// <typeparam name="T">The target state type</typeparam>
    /// <returns>True if the transition was successful; false otherwise</returns>
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

    /// <summary>
    /// Checks if the current state matches the specified type
    /// </summary>
    public bool CurrentStateIs<T>() where T : State, new()
    {
        return _currentState.GetType() == typeof(T);
    }

    /// <summary>
    /// Determines if transitioning to the specified state type is allowed from the current state
    /// </summary>
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

    private void UpdateStateMachine()
    {
        if (IsStateMachineInitialized())
        {
            _currentState.OnUpdate();
        }
    }

}
