using UnityEngine;
using UnityEngine.UI;

public class Settings
{
    public float m_mouseSensitivity = 1;
    public float m_volume = 1;
}

public class SettingsManager : Manager
{
    public Settings m_settings;
    public Slider m_sensitivitySlider;
    public Slider m_volumeSlider;
    public override void Deinitialize()
    {
    }

    public override void Initialize()
    {
        m_settings = new Settings();
        LoadSettings();
    }

    public override void UpdateManager()
    {
    }

    public void LoadSettings()
    {

    }

    public void SetSensityivitySlider(Slider slider)
    {
        m_sensitivitySlider = slider;

        m_sensitivitySlider.value = m_settings.m_mouseSensitivity;
    }

    public void UpdateSensitivitySliderValue()
    {
        if (m_sensitivitySlider != null)
        {
            m_settings.m_mouseSensitivity = m_sensitivitySlider.value;
        }
    }


    public void SetVolumeSlider(Slider slider)
    {
        m_volumeSlider = slider;

        m_volumeSlider.value = m_settings.m_volume;
    }

    public void UpdateVolumeSliderValue()
    {
        if (m_volumeSlider != null)
        {
            m_settings.m_volume = m_volumeSlider.value;
        }
    }
}
