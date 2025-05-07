using UnityEngine;

/// <summary>
/// Abstract base class for all game system managers
/// </summary>
public abstract class Manager
{
    public abstract void Initialize();
    public abstract void Deinitialize();
}
