using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public static GameManager GetInstance() 
    { 
        return _instance; 
    }
}
