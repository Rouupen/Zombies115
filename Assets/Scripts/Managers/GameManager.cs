using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Central manager that holds references to key game systems, managers, and player-related data.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Scriptable Objects
    [Header("Scriptable Objects Data")]
    public SO_InputData m_inputData;
    public SO_CharacterStatesFilter m_characterStatesData;
    public SO_GameValues m_gameValues;
    public SO_WeaponsInGame m_weaponsInGame;
    #endregion

    #region Player
    public PlayerController m_playerController;
    #endregion

    #region Managers
    private static GameManager _instance;

    public InputManager m_inputManager { get { return _inputManager; } }
    private InputManager _inputManager;
    #endregion

    //TEMP - UI Manager
    [HideInInspector] public CrosshairController m_crosshairController;
    [HideInInspector] public AmmoController m_ammoController;

    #region StateMachines
    /// <summary>Delegate used to update all active state machines each frame.</summary>
    public delegate void UpdateStateMachines();
    /// <summary>Called every frame to update registered state machines</summary>
    public UpdateStateMachines m_updateStateMachines;
    #endregion

    [HideInInspector] public PlayerInput m_playerInput;


    private void Awake()
    {
        // Ensure only one instance of GameManager exists (Singleton pattern) 
        // Currently, duplicate instances are destroyed, but this behavior may change
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        m_playerInput = GetComponent<PlayerInput>();

        // TEMP - UI Manager
        m_crosshairController = m_playerController.GetComponentInChildren<CrosshairController>();
        m_ammoController = m_playerController.GetComponentInChildren<AmmoController>();

        // Initialize managers and the player
        InitializeManagers();
        m_playerController.Initizalize();
    }

    private void Update()
    {
        if (m_updateStateMachines != null) 
        {
            m_updateStateMachines();
        }
    }

    /// <summary>
    /// Returns the singleton instance of the GameManager
    /// </summary>
    public static GameManager GetInstance() 
    { 
        return _instance; 
    }

    /// <summary>
    /// Initializes all game-level managers
    /// </summary>
    private void InitializeManagers()
    {
        _inputManager = new InputManager();
        _inputManager.Initialize();
    }

    /// <summary>
    /// Cleans up and deinitializes managers on shutdown
    /// </summary>
    private void DeinitializeManagers()
    {
        _inputManager.Deinitialize();
    }
}
