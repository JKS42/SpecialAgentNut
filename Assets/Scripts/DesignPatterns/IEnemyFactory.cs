using UnityEngine;

public abstract class IEnemyFactory : ScriptableObject
{
    public abstract GameObject CreateEnemy(Vector3 position, Quaternion rotation);
}
