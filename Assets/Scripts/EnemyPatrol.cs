using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private float rotationSpeed = 720f;

    public WaypointLinkedList waypoints = new WaypointLinkedList();
    private WaypointNode currentNode;
    private NavMeshAgent agent;
    private PlayerRespawn currentTarget;
    private float nextAttackTime;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
        }

        transform.Rotate(0f, 180f, 0f);

        currentTarget = Object.FindFirstObjectByType<PlayerRespawn>();

        WaypointManager waypointManager = Object.FindFirstObjectByType<WaypointManager>();
        if (waypointManager != null)
        {
            waypoints = waypointManager.customList;
            currentNode = waypoints.Head;
            UpdateTarget();
        }
        else
        {
            Debug.LogWarning("EnemyPatrol could not find a WaypointManager in the scene.");
        }
    }

    void Update()
    {
        if (agent == null)
        {
            return;
        }

        if (currentTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distanceToPlayer <= detectionRadius)
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);
                FaceMovementDirection();

                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                    nextAttackTime = Time.time + attackCooldown;
                }

                return;
            }
        }

        agent.isStopped = false;
        UpdateTarget();
        FaceMovementDirection();
    }

    void UpdateTarget()
    {
        if (agent == null || waypoints == null || currentNode == null)
        {
            return;
        }

        agent.SetDestination(currentNode.Data.position);
    }

    private void AttackPlayer()
    {
        if (currentTarget == null)
        {
            return;
        }

        currentTarget.TakeDamage(attackDamage);
        SFXManager.Instance.PlaySound("EnemyAttack");
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

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy Patrol takes " + amount + " damage!");

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit a waypoint
        if (other.CompareTag("Waypoint"))
        {
            // Advance to the next linked waypoint, looping back to the head.
            currentNode = currentNode != null && currentNode.Next != null
                ? currentNode.Next
                : waypoints.Head;
            UpdateTarget();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentTarget = collision.gameObject.GetComponent<PlayerRespawn>();

            if (agent != null)
            {
                agent.isStopped = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentTarget = collision.gameObject.GetComponent<PlayerRespawn>();

            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentTarget = null;

            if (agent != null)
            {
                agent.isStopped = false;
            }

            UpdateTarget();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
