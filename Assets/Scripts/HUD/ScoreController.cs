using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScoreController : MonoBehaviour
{

    [HideInInspector] public int m_score;
    [HideInInspector] public int m_kills;
    [HideInInspector] public int m_downs;

    [SerializeField] private TextMeshProUGUI m_nameText;
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [SerializeField] private TextMeshProUGUI m_killsText;
    [SerializeField] private TextMeshProUGUI m_downsText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private void OnEnable()
    {
        m_score = GameManager.GetInstance().m_playerController.m_UIController.m_pointsController.GetPoints();

        m_scoreText.text = m_score.ToString();
        m_killsText.text = m_kills.ToString();
        m_downsText.text = m_downs.ToString();
    }
}
