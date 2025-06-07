using System;
using System.Collections;
using UnityEngine;

public class PowerUpManager : Manager
{
    private Coroutine _instaKillCoroutine;
    private Coroutine _doublePointsCoroutine;
    public override void Deinitialize()
    {
    }

    public override void Initialize()
    {
    }

    public override void UpdateManager()
    {
    }

    public bool TryToInstatiateRandomNewPowerUp(Vector3 position)
    {
        int num = UnityEngine.Random.Range(0, 10);
        if (num < 1)
        {
            InstatiateNewRandomPowerUp(position);
            return true;
        }
        return false;
    }

    public void InstatiateNewRandomPowerUp(Vector3 position)
    {
        int length = Enum.GetValues(typeof(PowerUps)).Length;
        int randomPowerUp = UnityEngine.Random.Range(0, length);
        InstantiatePowerUp((PowerUps)randomPowerUp, position);
    }

    public void InstantiatePowerUp(PowerUps powerUp, Vector3 position)
    {
        GameObject go = UnityEngine.Object.Instantiate(GameManager.GetInstance().m_gameValues.m_powerUpObject, position, Quaternion.identity);
        if (go.TryGetComponent<PowerUp>(out PowerUp powerUpComponent))
        {
            powerUpComponent.Initialize(powerUp);
        }
    }

    public void StartInstaKill()
    {
        if (_instaKillCoroutine != null)
        {
            GameManager.GetInstance().StopCoroutine(_instaKillCoroutine);
        }
        _instaKillCoroutine = GameManager.GetInstance().StartCoroutine(InstaKill());
    }

    private IEnumerator InstaKill()
    {
        GameManager.GetInstance().m_spawnManager.SetInstaKillAllEnemys(true);
        GameManager.GetInstance().m_playerController.m_UIController.m_powerUpController.AddPowerUp(PowerUps.InstaKill);
        yield return new WaitForSeconds(15);
        GameManager.GetInstance().m_playerController.m_UIController.m_powerUpController.RemovePowerUp(PowerUps.InstaKill);
        GameManager.GetInstance().m_spawnManager.SetInstaKillAllEnemys(false);
    }

    public void StartDoublePoints()
    {
        if (_doublePointsCoroutine != null)
        {
            GameManager.GetInstance().StopCoroutine(_doublePointsCoroutine);
        }
        _doublePointsCoroutine = GameManager.GetInstance().StartCoroutine(DoublePoints());
    }

    private IEnumerator DoublePoints()
    {
        GameManager.GetInstance().m_playerController.m_UIController.m_pointsController.SetPointsMult(2);
        GameManager.GetInstance().m_playerController.m_UIController.m_powerUpController.AddPowerUp(PowerUps.DoublePoints);
        yield return new WaitForSeconds(15);
        GameManager.GetInstance().m_playerController.m_UIController.m_pointsController.SetPointsMult(1);
        GameManager.GetInstance().m_playerController.m_UIController.m_powerUpController.RemovePowerUp(PowerUps.DoublePoints);

    }
}
