using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WaypointNode
{
    public Transform Data; 
    public WaypointNode Next;      

    public WaypointNode (Transform data)
    {
        Data = data;
        Next = null;
    }
}

