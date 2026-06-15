using System.Collections.Generic;
using UnityEngine;

public class GraphNode : MonoBehaviour
{
[Header("Graph Info")]
    public string nodeName;

    [Tooltip("Other nodes directly connected to this one.")]
    public List<GraphNode> neighbors = new List<GraphNode>();

    private void Reset()
    {
        nodeName = gameObject.name;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.3f);

        if (neighbors == null)
            return;

        Gizmos.color = Color.white;

        foreach (GraphNode neighbor in neighbors)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }
}
