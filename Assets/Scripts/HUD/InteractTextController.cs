using TMPro;
using UnityEngine;

public class InteractTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_text;

    public void SetText(string text)
    {
        m_text.text = text;
    }

    public void RemoveText()
    {
        m_text.text = "";
    }
}
