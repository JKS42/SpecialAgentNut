using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private ScriptableObject scriptableObject;
    private IEnemyFactory enemyFactory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        enemyFactory = scriptableObject as IEnemyFactory;
    }
    public void SpawnEnemy()
    {
        IEnemy enemy = enemyFactory.CreateEnemy();
        enemy.Attack();
    }
    //void Start()
    //{
      //  SpawnEnemy();
    //}
}
