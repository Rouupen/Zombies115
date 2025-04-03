using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Scriptable Objects
    [Header("Scriptable Objects Data")]
    public SO_InputData m_inputData;
    public SO_CharacterStatesFilter m_characterStatesData;
    #endregion


    #region Managers
    private static GameManager _instance;

    public InputManager m_inputManager { get { return _inputManager; } }
    private InputManager _inputManager;
    #endregion

    #region StateMachines
    public delegate void UpdateStateMachines();
    public UpdateStateMachines m_updateStateMachines;
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

    private void Update()
    {
        if (m_updateStateMachines != null) 
        {
            m_updateStateMachines();
        }
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
