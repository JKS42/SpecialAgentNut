using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1f;

    public WaypointLinkedList waypoints = new WaypointLinkedList();
    private int currentIndex = 0;
    private NavMeshAgent agent;
    private PlayerRespawn currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        WaypointManager waypointManager = Object.FindFirstObjectByType<WaypointManager>();
        if (waypointManager != null)
        {
            waypoints = waypointManager.customList;
            UpdateTarget();
        }
        else
        {
            Debug.LogWarning("EnemyPatrol could not find a WaypointManager in the scene.");
        }
    }

    void UpdateTarget()
    {
        if (agent == null || waypoints == null || waypoints.Count == 0)
        {
            return;
        }

        Transform target = waypoints.GetByIndex(currentIndex);
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit a waypoint
        if (other.CompareTag("Waypoint"))
        {
            // Increment index and loop back to 0 if at the end
            currentIndex = (currentIndex + 1) % waypoints.Count;
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
}
