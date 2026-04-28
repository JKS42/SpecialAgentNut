using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    // Drag your sphere transforms here in the Inspector
    public Transform[] sceneWaypoints;

    public WaypointLinkedList customList = new WaypointLinkedList();

    void Awake()
    {
        // Transfer the waypoints from the Inspector array into Custom LinkedList
        foreach (Transform wp in sceneWaypoints)
        {
            customList.Add(wp);
        }
    }
}
