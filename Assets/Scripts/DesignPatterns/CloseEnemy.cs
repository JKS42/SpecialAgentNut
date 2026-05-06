using UnityEngine;
using UnityEngine.AI;

public class CloseEnemy : MonoBehaviour
{
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float patrolDelay = 2f;
    [SerializeField] private float patrolPointReachedDistance = 1f;

    private NavMeshAgent agent;
    private float nextPatrolTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Patrol();
    }
    void Update()
    {
        if (agent == null || agent.pathPending)
        {
            return;
        }
        if (agent.remainingDistance <= patrolPointReachedDistance && Time.time >= nextPatrolTime)
        {
            Patrol();
        }
    }

    public void Patrol()
    {
        if (agent == null)
        {
            return;
        }
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            nextPatrolTime = Time.time + patrolDelay;
        }
    }

    public void Attack()
    {
        Debug.Log("Close Enemy Attacks!");
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("Close Enemy takes " + amount + " damage!");
    }
}
