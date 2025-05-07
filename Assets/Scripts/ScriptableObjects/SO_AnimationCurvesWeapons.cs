using UnityEngine;

// Temporary ScriptableObject used for prototyping
// This will be removed

[CreateAssetMenu(fileName = "CurvesAnimationWeapons", menuName = "ScriptableObjects/CurvesAnimationWeapons", order = 3)]
public class SO_AnimationCurvesWeapons : ScriptableObject
{
    [Header("Walking")]
    public AnimationCurve m_bounceVerticalCurve;
    public AnimationCurve m_bounceHorizontalCurve;
    public AnimationCurve m_rotationLoopForwardCurve;
}
