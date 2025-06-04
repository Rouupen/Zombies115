using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{

    public CanvasGroup m_hudGO;
    public CanvasGroup m_scoreGO;

    public CrosshairController m_crosshairController;
    public AmmoController m_ammoController;
    public DamageController m_damageController;
    public PointsController m_pointsController;
    public InteractTextController m_interactTextController;
    public RoundsController m_roundsController;
    public PerksController m_perksController;
    public ScoreController m_scoreController;

    //private void Awake()
    //{
    //    m_crosshairController = GetComponentInChildren<CrosshairController>();
    //    m_ammoController = GetComponentInChildren<AmmoController>();
    //    m_damageController = GetComponentInChildren<DamageController>();
    //    m_pointsController = GetComponentInChildren<PointsController>();
    //    m_interactTextController = GetComponentInChildren<InteractTextController>();
    //    m_roundsController = GetComponentInChildren<RoundsController>();
    //    m_perksController = GetComponentInChildren<PerksController>();
    //    m_scoreController = m_scoreGO.gameObject.GetComponentInChildren<ScoreController>();
    //}

    void Start()
    {
        GameManager.GetInstance().m_inputManager.m_tab.started += StartTab;
        GameManager.GetInstance().m_inputManager.m_tab.canceled += EndTab;
    }

    private void OnDestroy()
    {
        GameManager.GetInstance().m_inputManager.m_tab.started -= StartTab;
        GameManager.GetInstance().m_inputManager.m_tab.canceled -= EndTab;
    }
    public void StartTab(InputAction.CallbackContext context)
    {
        m_hudGO.alpha = 0;
        m_scoreGO.alpha = 1;
        m_scoreGO.gameObject.SetActive(true);
    }
    public void EndTab(InputAction.CallbackContext context)
    {
        m_hudGO.alpha = 1;
        m_scoreGO.alpha = 0;
        m_scoreGO.gameObject.SetActive(false);
    }
}
