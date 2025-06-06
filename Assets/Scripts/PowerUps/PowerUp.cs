using UnityEngine;

public enum PowerUps
{
    MaxAmmo,
    Nuke,
    InstaKill,
    DoublePoints
}


public class PowerUp : MonoBehaviour
{
    private PowerUps _powerUp;
    private float _time = 15;
    private float _currentTime = 0;

    public void Initialize(PowerUps powerUp)
    {
        _powerUp = powerUp;
        if (GameManager.GetInstance().m_gameValues.m_powerUpModelDictionary.TryGetValue(powerUp, out GameObject model))
        {
            Instantiate(model, transform);
        }
    }

    private void Update()
    {
        if (_currentTime >= _time)
        {
            Destroy(gameObject);
        }
        _currentTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            switch (_powerUp)
            {
                case PowerUps.MaxAmmo:
                    player.FillAmmoAllWeapons();
                    break;
                case PowerUps.Nuke:
                    GameManager.GetInstance().m_spawnManager.KillAllActiveEnemys();
                    //player.m_UIController.m_pointsController.AddPoints(400);
                    break;
                case PowerUps.InstaKill:
                    GameManager.GetInstance().m_powerUpManager.StartInstaKill();
                    break;
                case PowerUps.DoublePoints:
                    GameManager.GetInstance().m_powerUpManager.StartDoublePoints();
                    break;
                default:
                    break;
            }

            Destroy(gameObject);
        }
    }
}
