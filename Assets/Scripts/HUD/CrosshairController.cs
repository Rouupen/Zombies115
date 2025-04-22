using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Tooltip("Top, Right, Bottom, Left")]
    public List<RectTransform> m_corrshairObj;
    public Vector2 m_minAndMaxPosition;

    // Update is called once per frame
    void Update()
    {
        
    }
}
