using System.Collections;
using UnityEngine;

public class BlackScreenController : MonoBehaviour
{
    public CanvasGroup m_canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFadeActive(bool active = true)
    {
        m_canvasGroup.alpha = active == true ? 1 : 0;
    }

    public IEnumerator FadeAnimation(bool active = true)
    {
        float time = 1;
        float currentTtime = 0;

        float start = active == true ? 1 : 0;
        float end = active == false ? 1 : 0;

        while (currentTtime <= time)
        {
            m_canvasGroup.alpha = Mathf.Lerp(start, end, currentTtime / time);
            currentTtime += Time.deltaTime;
            yield return null;
        }
        m_canvasGroup.alpha = end;
    }
}
