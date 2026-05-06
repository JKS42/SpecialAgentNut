using UnityEngine;
using UnityEngine.AI;

public class CloseEnemy : MonoBehaviour
{
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float patrolDelay = 2f;
    [SerializeField] private float patrolPointReachedDistance = 1f;
    [SerializeField] private float attackRadius = 5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask playerLayer;

    private NavMeshAgent agent;
    private float nextPatrolTime;
    private float nextAttackTime;
    private bool wasPlayerInRange;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Patrol();
    }
    void Update()
    {
        if (agent == null)
        {
            return;
        }

        bool isPlayerInRange = IsPlayerInAttackRadius();

        // Attack immediately when the player first enters range.
        if (isPlayerInRange && !wasPlayerInRange)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
        // Continue attacking while the player stays in range (cooldown gated).
        else if (isPlayerInRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }

        wasPlayerInRange = isPlayerInRange;

        if (isPlayerInRange || agent.pathPending)
        {
            return;
        }

        if (agent.remainingDistance <= patrolPointReachedDistance && Time.time >= nextPatrolTime)
        {
            Patrol();
        }
    }

    private bool IsPlayerInAttackRadius()
    {
        return Physics.CheckSphere(transform.position, attackRadius, playerLayer);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
