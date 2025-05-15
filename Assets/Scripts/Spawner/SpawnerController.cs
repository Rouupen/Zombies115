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
    private int _maxSpawned = 24;

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
                m_enemyList.Add(enemyClass);
            }
        }

        InitializeNewRound(10);
    }


    private void Update()
    {
        if (_currentSpawned > _totalToSpawn)
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
        for (int i = 0; i < m_enemyList.Count; i++)
        {
            if (!m_enemyList[i].gameObject.activeInHierarchy)
            {
                _currentSpawned++;
                m_enemyList[i].gameObject.SetActive(true);
                SpawnPointData spawnPoint = m_spawnPointsData[Random.Range(0, m_spawnPointsData.Count)];
                m_enemyList[i].transform.position = spawnPoint.transform.position;
                m_enemyList[i].InitializeEnemy(spawnPoint.m_spawnType);
                break;
            }
        }
    }


    public void SetTotalToSpawn(int totalToSpawn)
    {
        _totalToSpawn = totalToSpawn;
    }

    public void InitializeNewRound(int totalToSpawn)
    {
        SetTotalToSpawn(totalToSpawn);
        _currentSpawned = 0;
    }
}
