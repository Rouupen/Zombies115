using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpController : MonoBehaviour
{
    public List<Image> m_images;

    private Dictionary<PowerUps, Image> m_perksImg;

    private void Awake()
    {
        m_perksImg = new Dictionary<PowerUps, Image>();
    }

    public void AddPowerUp(PowerUps powerUp)
    {
        if (m_perksImg.ContainsKey(powerUp))
        {
            return;
        }

        Image image = null;

        for (int i = 0; i < m_images.Count; i++)
        {
            if (!m_images[i].gameObject.activeInHierarchy)
            {
                image = m_images[i];
                break;
            }
        }

        if (image != null)
        {
            m_perksImg.Add(powerUp, image);
            image.sprite = GameManager.GetInstance().m_gameValues.m_powerUpSpriteDictionary.GetValueOrDefault(powerUp);
            image.gameObject.SetActive(true);
        }
    }

    public void RemovePowerUp(PowerUps powerUp)
    {
        if (m_perksImg.TryGetValue(powerUp, out Image image))
        {
            image.sprite = null;
            image.gameObject.SetActive(false);
            m_perksImg.Remove(powerUp);
        }
    }
}
