using System.Collections.Generic;
using UnityEngine;

public class GraphRoute : MonoBehaviour
{
 [Header("Route Search")]
    public GraphNode startNode;
    public GraphNode goalNode;

    [Header("Debug")]
    public bool findRouteOnStart = true;

    private void Start()
    {
        if (findRouteOnStart)
        {
            List<GraphNode> path = FindShortestPathBFS(startNode, goalNode);

            if (path.Count == 0)
            {
                Debug.Log("No route found.");
            }
            else
            {
                Debug.Log("Shortest Route: " + FormatPath(path));
            }
        }
    }

    /// Breadth-First Search for shortest path in an unweighted graph.
    public List<GraphNode> FindShortestPathBFS(GraphNode start, GraphNode goal)
    {
        List<GraphNode> finalPath = new List<GraphNode>();

        if (start == null || goal == null)
        {
            Debug.LogWarning("Start node or goal node is missing.");
            return finalPath;
        }

        Queue<GraphNode> queue = new Queue<GraphNode>();
        HashSet<GraphNode> visited = new HashSet<GraphNode>();
        Dictionary<GraphNode, GraphNode> cameFrom = new Dictionary<GraphNode, GraphNode>();

        queue.Enqueue(start);
        visited.Add(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            GraphNode current = queue.Dequeue();

            if (current == goal)
            {
                // Rebuild path from goal back to start
                GraphNode step = goal;

                while (step != null)
                {
                    finalPath.Insert(0, step);
                    step = cameFrom[step];
                }

                return finalPath;
            }

            foreach (GraphNode neighbor in current.neighbors)
            {
                if (neighbor == null)
                    continue;

                if (visited.Contains(neighbor))
                    continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        return finalPath;
    }

    private string FormatPath(List<GraphNode> path)
    {
        if (path == null || path.Count == 0)
            return "Empty";

        List<string> names = new List<string>();

        foreach (GraphNode node in path)
        {
            names.Add(node.nodeName);
        }

        return string.Join(" -> ", names);
    }
}
