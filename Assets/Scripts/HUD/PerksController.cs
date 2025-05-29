using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerksController : MonoBehaviour
{
    public List<Image> m_images;

    private Dictionary<Perks, Image> m_perksImg;
    
    public void AddPerk(Perks perk)
    {
        if (m_perksImg.ContainsKey(perk))
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
            m_perksImg.Add(perk, image);
            image.sprite = null; //CHANGE
            image.gameObject.SetActive(true);
        }
    }

    public void RemovePerk(Perks perk)
    {
        if (m_perksImg.TryGetValue(perk, out Image image))
        {
            image.sprite = null;
            image.gameObject.SetActive(false);
            m_perksImg.Remove(perk);
        }
    }

    public void RemoveAllPerks()
    {
        foreach(Image image in m_perksImg.Values)
        { 
            image.sprite = null;
            image.gameObject.SetActive(false);
        }

        m_perksImg.Clear();
    }
}
