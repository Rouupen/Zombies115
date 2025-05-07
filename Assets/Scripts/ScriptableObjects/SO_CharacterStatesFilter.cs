using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Defines which character states can transition to which others. Used to configure the state machine's valid transitions.
/// </summary>
[CreateAssetMenu(fileName = "CharacterStatesFilter", menuName = "ScriptableObjects/CharacterStatesFilter", order = 2)]
public class SO_CharacterStatesFilter : SO_StatesFilter
{
    [Header("---Character states info---")]
    public List<AllowedStates> m_statesInfo;

    /// <summary>
    /// Converts the enum-based character state transition list into a list of class types used at runtime
    /// </summary>
    /// <returns>A list of AllowedStatesFilter containing class types for state transitions</returns>
    public override List<AllowedStatesFilter> GetStatesData()
    {
        List<AllowedStatesFilter> data = new List<AllowedStatesFilter>();

        for (int i = 0; i < m_statesInfo.Count; i++)
        {
            AllowedStatesFilter allowedStatesFilter = new AllowedStatesFilter();
            Type mainType = Assembly.GetExecutingAssembly().GetType(m_statesInfo[i].m_mainState.ToString());
            
            if (mainType == null)
            {
                Debug.LogWarning($"Main character state not found: {m_statesInfo[i].m_mainState.ToString()}");
                continue;
            }
            
            allowedStatesFilter.m_mainState = mainType;
            allowedStatesFilter.m_allowedStates = new List<Type>();

            for (int j = 0; j < m_statesInfo[i].m_allowedStates.Count; j++)
            {
                Type valueType = Assembly.GetExecutingAssembly().GetType(m_statesInfo[i].m_allowedStates[j].ToString());

                if (valueType == null)
                {
                    Debug.LogWarning($"Allowed character state for {m_statesInfo[i].m_mainState} not found: {m_statesInfo[i].m_allowedStates[j]}");
                    continue;
                }
                allowedStatesFilter.m_allowedStates.Add(valueType);
            }

            data.Add(allowedStatesFilter);
        }
        return data;
    }
}

[System.Serializable]
public struct AllowedStates
{
    public CharacterStates m_mainState;
    public List<CharacterStates> m_allowedStates;
}