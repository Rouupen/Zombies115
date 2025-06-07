using System;
using UnityEngine;


//TEMP- Now is only for surtvival
public class GameModeManager : Manager
{
    private int _currentRound;
    private float _currentEnemyHealth;
    private Vector2 _currentEnemySpeed;
    public override void Deinitialize()
    {
        throw new System.NotImplementedException();
    }

    public override void Initialize()
    {
        _currentEnemyHealth = GameManager.GetInstance().m_gameValues.m_enemyStartHealth;
        StartNextRound();
    }

    public override void UpdateManager()
    {
        throw new System.NotImplementedException();
    }

    public float GetCurrentEnemyHealth()
    {
        return _currentEnemyHealth;
    }

    public float GetRandomSpeed()
    {
        return UnityEngine.Random.Range(_currentEnemySpeed.x, _currentEnemySpeed.y);
    }

    public void StartNextRound()
    {
        SO_GameValues gameValues = GameManager.GetInstance().m_gameValues;

        if (gameValues.m_enemyHealthAddNextRound.Length > _currentRound) 
        {
            _currentEnemyHealth += gameValues.m_enemyHealthAddNextRound[_currentRound];
        }
        else
        {
            _currentEnemyHealth *= gameValues.m_enemyExponentialHealth;
        }
        if (gameValues.m_enemyMinMaxSpeed.Length > _currentRound)
        {
            _currentEnemySpeed = gameValues.m_enemyMinMaxSpeed[_currentRound];
        }
        else
        {
            _currentEnemySpeed = gameValues.m_enemyMinMaxSpeedDefault;
        }

        

        _currentRound++;
        int enemysToSpawn = Mathf.RoundToInt(0.0842f * Mathf.Pow(_currentRound, 2) + 0.1954f * _currentRound + 22.05f);

        if (_currentRound < 10 )
        {
            enemysToSpawn /= 3;
        }
        //https://denkirson.proboards.com/thread/2555/zombie-stats-updated-october-11

        GameManager.GetInstance().m_spawnManager.InitializeNewRound(enemysToSpawn);
        GameManager.GetInstance().m_playerController.m_UIController.m_roundsController.SetCurrentRound(_currentRound);
    }
}
