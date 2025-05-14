using UnityEngine;

public class InteractableCostPointsBase : MonoBehaviour, IInteractable
{
    public string m_text;
    public int m_costPoints = 500;
    public Collider m_collider;


    public virtual bool Interact(PlayerController interactor)
    {
        //TEMP
        bool pointsRemoved = GameManager.GetInstance().m_pointsController.RemovePoints(m_costPoints);

        m_collider.enabled = !pointsRemoved;

        return pointsRemoved;
    }
    public virtual bool ShowInteract(PlayerController interactor)
    {
        string text = m_text + $"\n Cost: {m_costPoints}";
        GameManager.GetInstance().m_interactTextController.SetText(text);
        return true;
    }

    public virtual bool HideInteract(PlayerController interactor)
    {
        GameManager.GetInstance().m_interactTextController.RemoveText();
        return true;
    }
}
