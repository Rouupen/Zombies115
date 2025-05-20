using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public List<GameObject> m_enemys;
    public List<SpawnPointData> m_spawnPointsData;

    private List<Enemy> m_enemyList;
    private float timeSpawn = 3;
    private float currentTime = 0;

    private int _totalToSpawn;
    private int _currentSpawned;

    private int _maxSpawnedSameTime = 24;
    private int _currentSpawnedSameTime = 0;

    void Start()
    {
        m_enemyList = new List<Enemy>();

        foreach (GameObject enemy in m_enemys)
        {
            for (int i = 0; i < 25; i++)
            {
                GameObject goEnemy = Instantiate(enemy, transform);
                goEnemy.SetActive(false);
                Enemy enemyClass = goEnemy.GetComponent<Enemy>();
                enemyClass.SetTarget(GameManager.GetInstance().m_playerController.gameObject);
                enemyClass.GetComponent<EntityHealth>().m_onDeath += ZombieDead;
                m_enemyList.Add(enemyClass);
            }
        }

        //TEMP
        InitializeNewRound(100);
    }


    private void Update()
    {
        if (_currentSpawned > _totalToSpawn || _currentSpawnedSameTime >= _maxSpawnedSameTime)
        {
            return;
        }
        if (currentTime >= timeSpawn)
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
        m_enemyList[index].gameObject.SetActive(true);
        SpawnPointData spawnPoint = m_spawnPointsData[Random.Range(0, m_spawnPointsData.Count)];
        m_enemyList[index].transform.position = spawnPoint.transform.position;
        m_enemyList[index].InitializeEnemy(spawnPoint.m_spawnType);
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
}
