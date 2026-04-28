using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public WaypointLinkedList waypoints = new WaypointLinkedList();
    private int currentIndex = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Setup initial target
        waypoints = Object.FindFirstObjectByType<WaypointManager>().customList;
        UpdateTarget();
    }

    void UpdateTarget()
    {
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
}
