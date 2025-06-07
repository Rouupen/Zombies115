using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ApplicationManager : MonoBehaviour
{
    private static ApplicationManager _instance;
    public GameObject m_settingsMenu;
    public SoundManager m_soundManager { get { return _soundManager; } }
    private SoundManager _soundManager;

    public LoadSceneManager m_loadSceneManager { get { return _loadSceneManager; } }
    private LoadSceneManager _loadSceneManager;
    
    public SettingsManager m_settingsManager { get { return _settingsManager; } }
    private SettingsManager _settingsManager;


    private void Awake()
    {
        // Ensure only one instance of ApplicationManager exists (Singleton pattern) 
        // Currently, duplicate instances are destroyed
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeManagers();
    }
    public static ApplicationManager GetInstance()
    {
        return _instance;
    }
    public void InitializeManagers()
    {
        _soundManager = new SoundManager();
        _soundManager.Initialize();

        _loadSceneManager = new LoadSceneManager();
        _loadSceneManager.Initialize();

        _settingsManager = new SettingsManager();
        _settingsManager.Initialize();
    }

    public void LoadScene(string scene)
    {
        _loadSceneManager.LoadScene(scene);
    }

    public void CloseAplication()
    {
        Application.Quit();
    }

    public void SetSensitivitySlider(Slider slider)
    {
        _settingsManager.SetSensityivitySlider(slider);
    }
    public void UpdateSensitivitySliderValue()
    {
        _settingsManager.UpdateSensitivitySliderValue();
    }

    public void SetVolumeSlider(Slider slider)
    {
        _settingsManager.SetVolumeSlider(slider);
    }
    public void UpdateVolumeSliderValue()
    {
        _settingsManager.UpdateVolumeSliderValue();
    }

    public void ShowSettings(bool show)
    {
        m_settingsMenu.SetActive(show);
        Time.timeScale = show == true ? 0.01f : 1.0f;
    }

    public void ShowSettings(InputAction.CallbackContext context)
    {
        Time.timeScale = !m_settingsMenu.activeInHierarchy == true ? 0.0f : 1.0f;
        Cursor.lockState = !m_settingsMenu.activeInHierarchy == true ? CursorLockMode.None : CursorLockMode.Locked;
        m_settingsMenu.SetActive(!m_settingsMenu.activeInHierarchy);


    }
}
