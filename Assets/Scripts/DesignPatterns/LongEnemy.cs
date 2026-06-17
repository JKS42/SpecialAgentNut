using UnityEngine;
using UnityEngine.AI;

public class LongEnemy : MonoBehaviour
{
    [SerializeField] private float patrolDelay = 2f;
    [SerializeField] private float patrolPointReachedDistance = 1f;
    [SerializeField] private float attackRadius = 12f;
    [SerializeField] private float attackCooldown = 1.25f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 18f;
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private float rotationSpeed = 720f;

    public WaypointLinkedList waypoints = new WaypointLinkedList();

    private NavMeshAgent agent;
    private int currentHealth;
    private float nextPatrolTime;
    private float nextAttackTime;
    private Transform currentTarget;
    private WaypointNode currentNode;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
        }

        WaypointManager waypointManager = Object.FindFirstObjectByType<WaypointManager>();
        if (waypointManager != null)
        {
            waypoints = waypointManager.customList;
            currentNode = waypoints.Head;
            Patrol();
        }
        else
        {
            Debug.LogWarning("LongEnemy could not find a WaypointManager in the scene.");
        }
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

            FaceMovementDirection();
            return;
        }

        if (agent.pathPending)
        {
            FaceMovementDirection();
            return;
        }

        if (agent.remainingDistance <= patrolPointReachedDistance && Time.time >= nextPatrolTime)
        {
            AdvanceWaypoint();
            Patrol();
        }

        FaceMovementDirection();
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
        if (agent == null || waypoints == null || currentNode == null)
        {
            return;
        }

        agent.SetDestination(currentNode.Data.position);
        nextPatrolTime = Time.time + patrolDelay;
    }

    private void AdvanceWaypoint()
    {
        if (waypoints == null || waypoints.Head == null)
        {
            return;
        }

        currentNode = currentNode != null && currentNode.Next != null
            ? currentNode.Next
            : waypoints.Head;
    }

    private void FaceMovementDirection()
    {
        if (agent == null)
        {
            return;
        }

        Vector3 direction = agent.desiredVelocity.sqrMagnitude > 0.01f
            ? agent.desiredVelocity
            : agent.velocity;

        direction.y = 0f;
        if (direction.sqrMagnitude <= 0.01f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
        SFXManager.Instance.PlaySound("EnemyAttack");
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Long Enemy takes " + amount + " damage!");

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
