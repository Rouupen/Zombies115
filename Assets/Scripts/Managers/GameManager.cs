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
    
    public ZoneManager m_zoneManager { get { return _zoneManager; } }
    private ZoneManager _zoneManager;
    
    public SpawnManager m_spawnManager { get { return _spawnManager; } }
    private SpawnManager _spawnManager;
    
    public GameModeManager m_gameModeManager { get { return _gameModeManager; } }
    private GameModeManager _gameModeManager;
    
    public PowerUpManager m_powerUpManager { get { return _powerUpManager; } }
    private PowerUpManager _powerUpManager;
    #endregion


    
    #region StateMachines
    /// <summary>Delegate used to update all active state machines each frame.</summary>
    public delegate void UpdateStateMachines();
    /// <summary>Called every frame to update registered state machines</summary>
    public UpdateStateMachines m_updateStateMachines;

    public delegate void UpdateManagers();
    /// <summary>Called every frame to update registered state machines</summary>
    public UpdateManagers m_updateManagers;
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


        // Initialize managers and the player
        InitializeManagers();
        m_playerController.Initizalize();
    }

    private void Update()
    {
        if (m_updateManagers != null)
        {
            m_updateManagers();
        }

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

        _zoneManager = new ZoneManager();
        _zoneManager.Initialize();
        
        _spawnManager = new SpawnManager();
        _spawnManager.Initialize();

        _gameModeManager = new GameModeManager();
        _gameModeManager.Initialize();

        _powerUpManager = new PowerUpManager();
        _powerUpManager.Initialize();
    }

    /// <summary>
    /// Cleans up and deinitializes managers on shutdown
    /// </summary>
    private void DeinitializeManagers()
    {
        _inputManager.Deinitialize();
        _zoneManager.Deinitialize();
        _spawnManager.Deinitialize();
        _gameModeManager.Deinitialize();
        _powerUpManager.Deinitialize();
    }
}
