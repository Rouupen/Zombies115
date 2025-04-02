using System;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "CharacterStatesFilter", menuName = "ScriptableObjects/CharacterStatesFilter", order = 2)]
public abstract class SO_StatesFilter : ScriptableObject
{
    public abstract Dictionary<State, Type> GetStatesDictionary();
}
