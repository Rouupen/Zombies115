using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public class CrosshairController : MonoBehaviour
{
    [Tooltip("Top, Right, Bottom, Left")]
    public List<RectTransform> m_corrshairObj;
    public Vector2 m_minAndMaxPosition;
    public CanvasGroup m_canvasGroup;

    private Color _currentColor = Color.white;

    private float m_speed = 10;
    private float m_fadeTime = 0.15f;
    private float m_rotateTime = 0.15f;

    private float m_currentPostion = 9.5f;
    private float m_lastPostion = 9.5f;
    private float m_currentCrosshairPostion;

    private bool m_isActive = true;

    private Coroutine m_showOrHideCrosshair;
    private Coroutine m_rotateCrosshair;
    // Update is called once per frame
    void Update()
    {
        float newValue = Mathf.Lerp(m_currentCrosshairPostion, m_currentPostion, Time.deltaTime * m_speed);
        newValue = Mathf.Clamp(newValue, m_minAndMaxPosition.x, m_minAndMaxPosition.y);
        m_corrshairObj[0].anchoredPosition = new Vector2(0, newValue);
        m_corrshairObj[1].anchoredPosition = new Vector2(newValue, 0);
        m_corrshairObj[2].anchoredPosition = new Vector2(0, -newValue);
        m_corrshairObj[3].anchoredPosition = new Vector2(-newValue, 0);

        m_currentCrosshairPostion = newValue;
    }


    public void SetCurrentAccuracy(float value)
    {
        Vector2 minMaxAccuracy = GameManager.GetInstance().m_gameValues.m_minMaxAccuracy;
        //Starts in 0
        minMaxAccuracy.y -= minMaxAccuracy.x;
        value -= minMaxAccuracy.x;

        float lerpValue = Mathf.Lerp(m_minAndMaxPosition.y, m_minAndMaxPosition.x, value / minMaxAccuracy.y);

        if (m_isActive)
        {
            m_currentPostion = lerpValue;
        }
        m_lastPostion = lerpValue;
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
        m_currentPostion = show ? m_lastPostion : 0;
        m_isActive = show;

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


    public void RotateCrosshair(float time)
    {
        if (m_rotateCrosshair != null)
        {
            StopCoroutine(m_rotateCrosshair);
        }
        m_rotateCrosshair = StartCoroutine(RotateCrosshairAnim(time));
    }


    private IEnumerator RotateCrosshairAnim(float time)
    {

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 rotation = new Vector3();

        float currentTime = 0;
        float start = rectTransform.localRotation.z;
        float end = 90f;

        while (currentTime <= time)
        {
            rotation.z = Mathf.Lerp(start, end, currentTime / m_rotateTime);
            rectTransform.localRotation = Quaternion.Euler(rotation);

            currentTime += Time.deltaTime;
            yield return null;
        }
        rotation.z = end;
        rectTransform.localRotation = Quaternion.Euler(rotation);
    }


    public void SetColor(Color color)
    {
        if (_currentColor == color)
        {
            return;
        }

        foreach (RectTransform rect in m_corrshairObj)
        {
            rect.GetComponent<UnityEngine.UI.Image>().color = color;
        }

        _currentColor = color;
    }
}
