using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for all states filters
/// </summary>
public abstract class SO_StatesFilter : ScriptableObject
{
    public abstract List<AllowedStatesFilter> GetStatesData();
}
