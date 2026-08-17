using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public GraphLoader graphLoader;
    public string startNodeId = "MainGate";
    public string endNodeId = "Library";

    public List<Node> FindPath()
    {
        if (graphLoader == null)
        {
            Debug.LogError("GraphLoader not assigned!");
            return new List<Node>();
        }

        if (!graphLoader.nodeMap.ContainsKey(startNodeId))
        {
            Debug.LogError("Start node not found: " + startNodeId);
            return new List<Node>();
        }

        if (!graphLoader.nodeMap.ContainsKey(endNodeId))
        {
            Debug.LogError("End node not found: " + endNodeId);
            return new List<Node>();
        }

        Node startNode = graphLoader.nodeMap[startNodeId];
        Node endNode = graphLoader.nodeMap[endNodeId];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        openSet.Add(startNode);
        gScore[startNode] = 0f;
        fScore[startNode] = Heuristic(startNode, endNode);

        while (openSet.Count > 0)
        {
            Node current = GetLowestF(openSet, fScore);

            if (current == endNode)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Node neighbor in current.neighbors)
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeG = gScore.ContainsKey(current)
                    ? gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position)
                    : float.MaxValue;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, endNode);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        Debug.LogWarning("No path found between " + startNodeId + " and " + endNodeId);
        return new List<Node>();
    }

    float Heuristic(Node a, Node b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    Node GetLowestF(List<Node> openSet, Dictionary<Node, float> fScore)
    {
        Node lowest = openSet[0];
        foreach (Node n in openSet)
        {
            if (fScore.ContainsKey(n) && fScore[n] < fScore[lowest])
                lowest = n;
        }
        return lowest;
    }

    List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> path = new List<Node>();
        while (cameFrom.ContainsKey(current))
        {
            path.Insert(0, current);
            current = cameFrom[current];
        }
        path.Insert(0, current);
        return path;
    }
}


// using System.Collections.Generic;
// using UnityEngine;

// public class PathFinder : MonoBehaviour
// {
//     public GraphLoader graphLoader;
//     public string startNodeId = "MainGate";
//     public string endNodeId = "Library";

//     public List<Node> FindPath()
//     {
//         Node startNode = graphLoader.nodeMap[startNodeId];
//         Node endNode = graphLoader.nodeMap[endNodeId];

//         // A* setup
//         List<Node> openSet = new List<Node>();
//         HashSet<Node> closedSet = new HashSet<Node>();
//         Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
//         Dictionary<Node, float> gScore = new Dictionary<Node, float>();
//         Dictionary<Node, float> fScore = new Dictionary<Node, float>();

//         openSet.Add(startNode);
//         gScore[startNode] = 0f;
//         fScore[startNode] = Heuristic(startNode, endNode);

//         while (openSet.Count > 0)
//         {
//             // Get node with lowest fScore
//             Node current = GetLowestF(openSet, fScore);

//             if (current == endNode)
//                 return ReconstructPath(cameFrom, current);

//             openSet.Remove(current);
//             closedSet.Add(current);

//             foreach (Node neighbor in current.neighbors)
//             {
//                 if (closedSet.Contains(neighbor)) continue;

//                 float tentativeG = gScore.ContainsKey(current) 
//                     ? gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position)
//                     : float.MaxValue;

//                 if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
//                 {
//                     cameFrom[neighbor] = current;
//                     gScore[neighbor] = tentativeG;
//                     fScore[neighbor] = tentativeG + Heuristic(neighbor, endNode);

//                     if (!openSet.Contains(neighbor))
//                         openSet.Add(neighbor);
//                 }
//             }
//         }

//         Debug.LogWarning("No path found!");
//         return new List<Node>();
//     }

//     float Heuristic(Node a, Node b)
//     {
//         return Vector3.Distance(a.transform.position, b.transform.position);
//     }

//     Node GetLowestF(List<Node> openSet, Dictionary<Node, float> fScore)
//     {
//         Node lowest = openSet[0];
//         foreach (Node n in openSet)
//         {
//             if (fScore.ContainsKey(n) && fScore[n] < fScore[lowest])
//                 lowest = n;
//         }
//         return lowest;
//     }

//     List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
//     {
//         List<Node> path = new List<Node>();
//         while (cameFrom.ContainsKey(current))
//         {
//             path.Insert(0, current);
//             current = cameFrom[current];
//         }
//         path.Insert(0, current);
//         return path;
//     }
// }

