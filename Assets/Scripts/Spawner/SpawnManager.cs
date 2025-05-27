using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
//Manager
public class SpawnManager : Manager
{
    public List<SpawnPointData> m_spawnPointsData;

    private List<Enemy> m_enemyList;
    private float timeSpawn = 3;
    private float currentTime = 0;

    private int _totalToSpawn;
    private int _currentSpawned;

    private int _maxSpawnedSameTime = 24;
    private int _currentSpawnedSameTime = 0;

    private GameObject _enemysObject;

    public override void Initialize()
    {
        GameManager.GetInstance().m_updateManagers += UpdateManager;
        m_spawnPointsData = new List<SpawnPointData>();
        m_enemyList = new List<Enemy>();
        _enemysObject = GameManager.Instantiate(new GameObject(), GameManager.GetInstance().transform.position, Quaternion.identity);
        _enemysObject.name = "EnemysObject";
        foreach (GameObject enemy in GameManager.GetInstance().m_gameValues.m_enemys)
        {
            for (int i = 0; i < 25; i++)
            {
                GameObject goEnemy = GameManager.Instantiate(enemy, _enemysObject.transform, true);
                goEnemy.SetActive(false);
                Enemy enemyClass = goEnemy.GetComponent<Enemy>();
                enemyClass.SetTarget(GameManager.GetInstance().m_playerController.gameObject);
                enemyClass.GetComponent<EntityHealth>().m_onDeath += ZombieDead;
                m_enemyList.Add(enemyClass);
            }
        }
    }

    public override void Deinitialize()
    {
        GameManager.GetInstance().m_updateManagers -= UpdateManager;
    }

    public override void UpdateManager()
    {
        if (_currentSpawned > _totalToSpawn && _currentSpawnedSameTime == 0)
        {
            GameManager.GetInstance().m_gameModeManager.StartNextRound();
            currentTime = -8; //wait to start new round
            return;
        }

        if (_currentSpawnedSameTime >= _maxSpawnedSameTime || _currentSpawned > _totalToSpawn)
        {
            return;
        }
        if (currentTime >= timeSpawn && m_spawnPointsData.Count > 0)
        {
            SpawnZombie();

            currentTime = 0;
        }
        currentTime += Time.deltaTime;
    }

    public void SpawnZombie()
    {
        int index = Random.Range(0, m_enemyList.Count);

        while (m_enemyList[index].gameObject.activeInHierarchy)
        {
            index = Random.Range(0, m_enemyList.Count);
        }

        _currentSpawned++;
        _currentSpawnedSameTime++;
        SpawnPointData spawnPoint = m_spawnPointsData[Random.Range(0, m_spawnPointsData.Count)];
        m_enemyList[index].transform.position = spawnPoint.transform.position;
        m_enemyList[index].gameObject.SetActive(true);
        m_enemyList[index].InitializeEnemy(spawnPoint.m_spawnType, GameManager.GetInstance().m_gameModeManager.GetCurrentEnemyHealth(), GameManager.GetInstance().m_gameModeManager.GetRandomSpeed());
    }


    public void SetTotalToSpawn(int totalToSpawn)
    {
        _totalToSpawn = totalToSpawn;
    }

    public void InitializeNewRound(int totalToSpawn)
    {
        SetTotalToSpawn(totalToSpawn);
        _currentSpawned = 0;
        _currentSpawnedSameTime = 0;
    }

    public void ZombieDead()
    {
        _currentSpawnedSameTime--;
    }

    public void AddSpawnPoint(SpawnPointData spawnPoint)
    {
        if (!m_spawnPointsData.Contains(spawnPoint))
        {
            m_spawnPointsData.Add(spawnPoint);
        }
    }

    public void RemoveSpawnPoint(SpawnPointData spawnPoint)
    {
        if (m_spawnPointsData.Contains(spawnPoint))
        {
            m_spawnPointsData.Remove(spawnPoint);
        }
    }

}
