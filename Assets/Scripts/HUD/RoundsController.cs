using System.Collections;
using TMPro;
using UnityEngine;

public class RoundsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_roundText;

    private Vector2 _roundTextPos;
    private void Awake()
    {
        _roundTextPos = m_roundText.rectTransform.localPosition;
        m_roundText.rectTransform.localPosition = Vector2.zero;
    }

    public void SetCurrentRound(int round)
    {
        m_roundText.text = round.ToString();
    }

    public IEnumerator StartRoundAnimation()
    {
        float time = 2;
        float currentTime = 0;

        while (currentTime <= time)
        {
            m_roundText.rectTransform.localPosition = Vector2.Lerp(Vector2.zero, _roundTextPos, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }

        m_roundText.rectTransform.localPosition = _roundTextPos;
    }
}
