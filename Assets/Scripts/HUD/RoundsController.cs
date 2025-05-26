using TMPro;
using UnityEngine;

public class RoundsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_roundText;

    public void SetCurrentRound(int round)
    {
        m_roundText.text = round.ToString();
    }
}
