using UnityEngine;
using UnityEngine.AI;

public class CloseEnemy : MonoBehaviour
{
    [SerializeField] private float patrolDelay = 2f;
    [SerializeField] private float patrolPointReachedDistance = 1f;
    [SerializeField] private float attackRadius = 5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private int maxHealth = 40;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private LayerMask playerLayer;

    public WaypointLinkedList waypoints = new WaypointLinkedList();

    private NavMeshAgent agent;
    private int currentHealth;
    private float nextPatrolTime;
    private float nextAttackTime;
    private bool wasPlayerInRange;
    private WaypointNode currentNode;

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
            Debug.LogWarning("CloseEnemy could not find a WaypointManager in the scene.");
        }
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

    private bool IsPlayerInAttackRadius()
    {
        return Physics.CheckSphere(transform.position, attackRadius, playerLayer);
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
        Debug.Log("Close Enemy Attacks!");
        SFXManager.Instance.PlaySound("EnemyAttack");

        Collider[] playersInRange = Physics.OverlapSphere(transform.position, attackRadius, playerLayer);
        foreach (Collider playerCollider in playersInRange)
        {
            PlayerRespawn playerHealth = playerCollider.GetComponentInParent<PlayerRespawn>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                break;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Close Enemy takes " + amount + " damage!");

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
