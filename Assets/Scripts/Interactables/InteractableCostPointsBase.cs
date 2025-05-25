using UnityEngine;

//Need refactor
public class InteractableCostPointsBase : MonoBehaviour, IInteractable
{
    public string m_text;
    public int m_costPoints = 500;
    public bool m_needToLook;
    public Collider m_collider;
    public Collider m_trigger;
    private bool _isActive = true;
    protected AreaController _areaController;

    public virtual bool Interact(PlayerController interactor)
    {
        //TEMP
        if (_isActive)
        {
            bool pointsRemoved = GameManager.GetInstance().m_pointsController.RemovePoints(m_costPoints);

            m_collider.enabled = !pointsRemoved;
            _isActive = !pointsRemoved;
            GameManager.GetInstance().m_interactTextController.RemoveText();

            return pointsRemoved;
        }

        return false;
    }

    public virtual bool Unlock()
    {
        //TEMP
        if (_isActive)
        {

            m_collider.enabled = false;
            _isActive = false;
            GameManager.GetInstance().m_interactTextController.RemoveText();

            return true;
        }

        return false;
    }

    public virtual bool ShowInteract(PlayerController interactor, bool look)
    {
        if (m_needToLook != look || !_isActive)
        {
            return false;
        }
        string text = m_text + $"\n Cost: {m_costPoints}";
        GameManager.GetInstance().m_interactTextController.SetText(text);
        return true;
    }

    public virtual bool HideInteract(PlayerController interactor, bool look)
    {
        if (m_needToLook != look || !_isActive)
        {
            return false;
        }
        GameManager.GetInstance().m_interactTextController.RemoveText();
        return true;
    }
    //TEMP
    private void OnTriggerEnter(Collider other)
    {
        if (m_needToLook)
        {
            return;
        }

        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            ShowInteract(player, false);
            player.m_characterInteraction.AddInteract(this);
        }
    }
    //TEMP
    private void OnTriggerExit(Collider other)
    {
        if (m_needToLook)
        {
            return;
        }

        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            HideInteract(player, false);
            player.m_characterInteraction.RemoveInteract(this);
        }
    }
    public void SetActive(bool active)
    {
        m_collider.enabled = active;
        _isActive = active;
    }

    public void AddArea(AreaController area)
    {
        _areaController = area;
    }
}
