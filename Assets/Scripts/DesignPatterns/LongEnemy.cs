using UnityEngine;
using UnityEngine.AI;

public class LongEnemy : MonoBehaviour
{
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float patrolDelay = 2f;
    [SerializeField] private float patrolPointReachedDistance = 1f;
    private NavMeshAgent agent;
    private float nextPatrolTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
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
        Debug.Log("Long Enemy Attacks!");
    }
    public void TakeDamage(int amount)
    {
        Debug.Log("Long Enemy takes " + amount + " damage!");
    }
}
