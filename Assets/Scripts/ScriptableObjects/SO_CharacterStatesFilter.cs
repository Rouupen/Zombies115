using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AllowedStates
{
    public CharacterStates.CharacterStates m_currentState;
    public List<CharacterStates.CharacterStates> m_allowedStates;
}
[CreateAssetMenu(fileName = "CharacterStatesFilter", menuName = "ScriptableObjects/CharacterStatesFilter", order = 2)]
public class SO_CharacterStatesFilter : SO_StatesFilter
{
    [SerializeField]
    public List<AllowedStates> m_statesInfo;

    public override Dictionary<State, Type> GetStatesDictionary()
    {
        new Dictionary<Type, List<Type>>(); //PROBAR ESTO
        return new Dictionary<State, Type>();
    }
}
