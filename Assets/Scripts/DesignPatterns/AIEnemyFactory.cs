using UnityEngine;

public class AIEnemyFactory : MonoBehaviour
{
    [SerializeField] private IEnemyFactory enemyFactory;
    [SerializeField] private Transform spawnPoint;

    public GameObject SpawnEnemy()
    {
        if (enemyFactory == null)
        {
            Debug.LogError("AIEnemyFactory needs an enemy factory assigned.");
            return null;
        }

        Transform currentSpawnPoint = spawnPoint != null ? spawnPoint : transform;
        return enemyFactory.CreateEnemy(currentSpawnPoint.position, currentSpawnPoint.rotation);
    }

    public void Attack()
    {
        
    }

    public void TakeDamage(int amount)
    {
        
    }
}
