using TMPro;
using UnityEngine;

public class PointsController : MonoBehaviour
{
    private int m_totalPoints;
    [SerializeField] private TextMeshProUGUI m_pointsText;

    private void Awake()
    {
        //TEMP
        AddPoints(500);
    }


    public void AddPoints(int points)
    {
        m_totalPoints += points;
        UpdateText();
    }

    public bool CanRemovePoints(int points)
    {
        return m_totalPoints >= points;
    }

    public bool RemovePoints(int points)
    {
        if (!CanRemovePoints(points))
        {
            return false;
        }

        m_totalPoints -= points;

        if (m_totalPoints < 0)
        {
            m_totalPoints = 0;
        }

        UpdateText();
        return true;
    }

    public void UpdateText()
    {
        m_pointsText.text = m_totalPoints.ToString();
    }
}
