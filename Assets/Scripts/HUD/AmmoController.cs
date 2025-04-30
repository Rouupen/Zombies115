using TMPro;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    public TextMeshProUGUI m_magazineText;
    public TextMeshProUGUI m_reserveText;


    public void UpdateAmmoHud(int magazine, int reserve)
    {
        m_magazineText.text = magazine.ToString();
        m_reserveText.text = reserve.ToString();
    }
}
