using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private IEnemyFactory enemyFactory;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnIntervalSeconds = 5f;
    [SerializeField] private int maxEnemies = 5;

    private float spawnTimer;
    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start()
    {
        spawnTimer = spawnIntervalSeconds;
        SpawnEnemy();
    }

    private void FixedUpdate()
    {
        CleanupDestroyedEnemies();

        if (spawnedEnemies.Count >= maxEnemies)
        {
            return;
        }

        spawnTimer -= Time.fixedDeltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnIntervalSeconds;
        }
    }

    public GameObject SpawnEnemy()
    {
        CleanupDestroyedEnemies();

        if (spawnedEnemies.Count >= maxEnemies)
        {
            return null;
        }

        if (enemyFactory == null)
        {
            Debug.LogError("EnemySpawner needs an enemy factory assigned.");
            return null;
        }

        Transform currentSpawnPoint = spawnPoint != null ? spawnPoint : transform;
        GameObject spawnedEnemy = enemyFactory.CreateEnemy(currentSpawnPoint.position, currentSpawnPoint.rotation);

        if (spawnedEnemy != null)
        {
            spawnedEnemies.Add(spawnedEnemy);
        }

        return spawnedEnemy;
    }

    private void CleanupDestroyedEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }
}
