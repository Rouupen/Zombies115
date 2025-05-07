using TMPro;
using UnityEngine;

/// <summary>
/// Controls and updates the player's ammo display in the HUD
/// </summary>
public class AmmoController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI m_magazineText;
    [SerializeField] private TextMeshProUGUI m_reserveText;

    /// <summary>
    /// Updates the ammo display with current magazine and reserve values
    /// </summary>
    /// <param name="magazine">Current bullets in the magazine.</param>
    /// <param name="reserve">Current reserve bullets.</param>
    public void UpdateAmmoHud(int magazine, int reserve)
    {
        if (m_magazineText != null)
        {
            m_magazineText.text = magazine.ToString();
        }

        if (m_reserveText != null)
        {
            m_reserveText.text = reserve.ToString();
        }
    }
}
