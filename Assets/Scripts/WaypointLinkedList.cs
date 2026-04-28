using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WaypointLinkedList
{
    private WaypointNode head;
    private int count = 0;

    public int Count => count;

    // Add a new waypoint to the end of the list
    public void Add(Transform waypoint)
    {
        WaypointNode newNode = new WaypointNode(waypoint);
        if (head == null)
        {
            head = newNode;
        }
        else
        {
            WaypointNode temp = head;
            while (temp.Next != null)
            {
                temp = temp.Next;
            }
            temp.Next = newNode;
        }
        count++;
    }

    // Get a waypoint's Transform by its index
    public Transform GetByIndex(int index)
    {
        if (index < 0 || index >= count) return null;

        WaypointNode current = head;
        for (int i = 0; i < index; i++)
        {
            current = current.Next;
        }
        return current.Data;
    }
}
