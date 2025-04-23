using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CrosshairController : MonoBehaviour
{
    [Tooltip("Top, Right, Bottom, Left")]
    public List<RectTransform> m_corrshairObj;
    public Vector2 m_minAndMaxPosition;
    public CanvasGroup m_canvasGroup;

    private float m_speed = 10;
    private float m_fadeTime = 0.15f;
    private float m_rotateTime = 0.15f;



    private float m_idleAccuracy = 95f;
    private float m_walkingAccuracy = 300f;

    private float m_currentPostion = 9.5f;
    private float m_currentCrosshairPostion;

    private Coroutine m_showOrHideCrosshair;
    private Coroutine m_rotateCrosshair;
    // Update is called once per frame
    void Update()
    {
        float newValue = Mathf.Lerp(m_currentCrosshairPostion, m_currentPostion, Time.deltaTime * m_speed);

        m_corrshairObj[0].anchoredPosition = new Vector2(0, newValue);
        m_corrshairObj[1].anchoredPosition = new Vector2(newValue, 0);
        m_corrshairObj[2].anchoredPosition = new Vector2(0, -newValue);
        m_corrshairObj[3].anchoredPosition = new Vector2(-newValue, 0);

        m_currentCrosshairPostion = newValue;
    }


    public void SetCurrentPosition(float value)
    {
        m_currentPostion = value;
    }

    public void ShowOrHideCrosshair(bool show = true, bool instantly = false)
    {
        if (m_showOrHideCrosshair != null)
        {
            StopCoroutine(m_showOrHideCrosshair);
        }
        m_showOrHideCrosshair = StartCoroutine(ShowOrHideCrosshairAnim(show, instantly));
    }


    private IEnumerator ShowOrHideCrosshairAnim(bool show = true, bool instantly = false)
    {
        m_currentPostion = show ? m_idleAccuracy : 0;

        if (instantly)
        {
            m_canvasGroup.alpha = show ? 1 : 0;
            yield break;
        }

        float currentTime = 0;
        float start = m_canvasGroup.alpha;
        float end = show ? 1 : 0;

        while (currentTime <= m_fadeTime)
        {
            m_canvasGroup.alpha = Mathf.Lerp(start, end, currentTime / m_fadeTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        m_canvasGroup.alpha = end;

    }


    public void RotateCrosshair()
    {
        if (m_rotateCrosshair != null)
        {
            StopCoroutine(m_rotateCrosshair);
        }
        m_rotateCrosshair = StartCoroutine(RotateCrosshairAnim());
    }


    private IEnumerator RotateCrosshairAnim()
    {

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 rotation = new Vector3();

        float currentTime = 0;
        float start = rectTransform.localRotation.z;
        float end = 90f;

        while (currentTime <= m_rotateTime)
        {
            rotation.z = Mathf.Lerp(start, end, currentTime/ m_rotateTime);
            rectTransform.localRotation = Quaternion.Euler(rotation);

            currentTime += Time.deltaTime;
            yield return null;
        }
        rotation.z = end;
        rectTransform.localRotation = Quaternion.Euler(rotation);
    }
}
