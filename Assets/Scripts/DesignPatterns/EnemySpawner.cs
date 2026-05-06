using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private IEnemyFactory enemyFactory;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxEnemies = 5;

    private int currentEnemies = 0;

    private void Start()
    {
        SpawnEnemy();
    }
    private void FixedUpdate()
    {
        if (currentEnemies >= maxEnemies)
        {
            return;
        }

        spawnInterval -= Time.fixedDeltaTime;
        if (spawnInterval <= 0)
        {
            SpawnEnemy();
            spawnInterval = 5f; 
        }
    }

    public GameObject SpawnEnemy()
    {
        if (currentEnemies >= maxEnemies)
        {
            return null;
        }

        if (enemyFactory == null)
        {
            Debug.Log("Doesn't work");
            return null;
        }

        Transform currentSpawnPoint = spawnPoint != null ? spawnPoint : transform;
        GameObject spawnedEnemy = enemyFactory.CreateEnemy(currentSpawnPoint.position, currentSpawnPoint.rotation);

        if (spawnedEnemy != null)
        {
            currentEnemies++;
        }

        return spawnedEnemy;
    }
}
