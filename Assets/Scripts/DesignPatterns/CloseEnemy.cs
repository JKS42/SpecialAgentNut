using UnityEngine;
using UnityEngine.AI;

public class CloseEnemy : MonoBehaviour
{
    [SerializeField] private float patrolDelay = 2f;
    [SerializeField] private float patrolPointReachedDistance = 1f;
    [SerializeField] private float attackRadius = 5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private LayerMask playerLayer;

    public WaypointLinkedList waypoints = new WaypointLinkedList();

    private NavMeshAgent agent;
    private float nextPatrolTime;
    private float nextAttackTime;
    private bool wasPlayerInRange;
    private WaypointNode currentNode;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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
            return;
        }

        if (agent.remainingDistance <= patrolPointReachedDistance && Time.time >= nextPatrolTime)
        {
            AdvanceWaypoint();
            Patrol();
        }
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
        Debug.Log("Close Enemy takes " + amount + " damage!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
