using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float detectionRadius = 8f;

    public WaypointLinkedList waypoints = new WaypointLinkedList();
    private WaypointNode currentNode;
    private NavMeshAgent agent;
    private PlayerRespawn currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
                return;
            }
        }

        agent.isStopped = false;
        UpdateTarget();
    }

    void UpdateTarget()
    {
        if (agent == null || waypoints == null || currentNode == null)
        {
            return;
        }

        agent.SetDestination(currentNode.Data.position);
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
            SFXManager.Instance.PlaySound("EnemyAttack");

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
