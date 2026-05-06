using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConcreteFactory", menuName = "Scriptable Objects/EnemyConcreteFactory")]
public class EnemyConcreteFactory : IEnemyFactory
{
    [SerializeField] private GameObject enemyPrefab;

    public override GameObject CreateEnemy(Vector3 position, Quaternion rotation)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Need a object to spawn");
            return null;
        }

        return Instantiate(enemyPrefab, position, rotation);
    }
}
