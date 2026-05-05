using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConcreteFactory", menuName = "Scriptable Objects/EnemyConcreteFactory")]
public class EnemyConcreteFactory : ScriptableObject, IEnemyFactory
{
    public GameObject enemyPrefab;
    public IEnemy CreateEnemy()
    {
        return Instantiate(enemyPrefab).GetComponent<IEnemy>();
    }
}
