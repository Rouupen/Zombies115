using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Tooltip("Top, Right, Bottom, Left")]
    public List<RectTransform> m_corrshairObj;
    public Vector2 m_minAndMaxPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
