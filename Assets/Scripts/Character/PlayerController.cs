using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController m_characterController;
    public CharacterMovement m_characterMovement;
    public CharacterLook m_characterLook;

    private void Start()
    {
        GameManager.GetInstance().m_playerController = this;
        GameManager.GetInstance().m_playerController = this;
    }
}
