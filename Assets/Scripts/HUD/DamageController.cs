using UnityEngine;

public class DamageController : MonoBehaviour
{
    public CanvasGroup m_canvasGroup;

    public void SetDamageBlend(float damage)
    {
        m_canvasGroup.alpha = damage;
    }

}
