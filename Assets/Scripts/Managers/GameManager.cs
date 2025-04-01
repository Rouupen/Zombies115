using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Scriptable Objects
    [Header("Scriptable Objects Data")]
    public SO_InputData m_inputData;
    #endregion


    #region Managers
    private static GameManager _instance;

    public InputManager m_inputManager { get { return _inputManager; } }
    private InputManager _inputManager;

    #endregion

    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        InitializeManagers();
    }

    public static GameManager GetInstance() 
    { 
        return _instance; 
    }

    private void InitializeManagers()
    {
        _inputManager = new InputManager();
        _inputManager.Initialize();
    }

    private void DeinitializeManagers()
    {
        _inputManager.Deinitialize();
    }
}
