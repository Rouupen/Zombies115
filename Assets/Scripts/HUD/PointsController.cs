using TMPro;
using UnityEngine;

public class PointsController : MonoBehaviour
{
    private int _totalPoints;
    [SerializeField] private TextMeshProUGUI m_pointsText;
    public float _pointsMult = 1;

    private void Start()
    {
        //TEMP
        AddPoints(5000);
    }


    public void AddPoints(int points)
    {
        _totalPoints += (int)(points * _pointsMult);
        UpdateText();
    }

    public bool CanRemovePoints(int points)
    {
        return _totalPoints >= points;
    }

    public bool RemovePoints(int points)
    {
        if (!CanRemovePoints(points))
        {
            return false;
        }

        _totalPoints -= points;

        if (_totalPoints < 0)
        {
            _totalPoints = 0;
        }

        UpdateText();
        return true;
    }

    public int GetPoints()
    {
        return _totalPoints;
    }
    public void UpdateText()
    {
        m_pointsText.text = _totalPoints.ToString();
        GameManager.GetInstance().m_playerController.m_UIController.m_scoreController.m_score = _totalPoints;
    }

    public void SetPointsMult(int mult)
    {
        _pointsMult = mult;
    }
}
