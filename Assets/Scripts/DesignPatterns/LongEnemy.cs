using UnityEngine;
using UnityEngine.AI;

public class LongEnemy : MonoBehaviour
{
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float patrolDelay = 2f;
    [SerializeField] private float patrolPointReachedDistance = 1f;
    [SerializeField] private float attackRadius = 12f;
    [SerializeField] private float attackCooldown = 1.25f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 18f;

    private NavMeshAgent agent;
    private float nextPatrolTime;
    private float nextAttackTime;
    private Transform currentTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Patrol();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent == null)
        {
            return;
        }

        currentTarget = FindTargetInRange();
        if (currentTarget != null)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }

            return;
        }

        if (agent.pathPending)
        {
            return;
        }

        if (agent.remainingDistance <= patrolPointReachedDistance && Time.time >= nextPatrolTime)
        {
            Patrol();
        }
    }

    private Transform FindTargetInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, playerLayer);
        if (hits.Length == 0)
        {
            return null;
        }

        return hits[0].transform;
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
        if (projectilePrefab == null || currentTarget == null)
        {
            return;
        }

        Transform spawnTransform = firePoint != null ? firePoint : transform;
        Vector3 direction = (currentTarget.position - spawnTransform.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, spawnTransform.position, Quaternion.LookRotation(direction));
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileRb.linearVelocity = direction * projectileSpeed;
        }

        Debug.Log("Long Enemy shoots at player!");
    }
    public void TakeDamage(int amount)
    {
        Debug.Log("Long Enemy takes " + amount + " damage!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
