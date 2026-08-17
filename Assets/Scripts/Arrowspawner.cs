using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrowspawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    public PathFinder pathFinder;
    public GraphLoader graphLoader;

    [Header("Node Visuals")]
    public float nodeRadius = 0.03f;
    public Color nodeColor = Color.cyan;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnAfterLoad());
    }

    IEnumerator SpawnAfterLoad()
    {
        yield return new WaitForSeconds(3f);
        SpawnPath();
    }

    public void RespawnPath()
    {
        foreach (GameObject obj in spawnedObjects)
            Destroy(obj);
        spawnedObjects.Clear();

        SpawnPath();
    }

    void SpawnPath()
    {
        List<Node> path = pathFinder.FindPath();

        if (path.Count == 0)
        {
            Debug.LogWarning("No path found!");
            return;
        }

        Debug.Log("Path found with " + path.Count + " nodes!");

        // Reset all nodes to yellow
        foreach (var key in graphLoader.nodeRenderers.Keys)
            graphLoader.nodeRenderers[key].material = graphLoader.yellowMaterial;

        // Highlight path nodes cyan
        foreach (Node node in path)
        {
            string nodeName = node.gameObject.name;
            if (graphLoader.nodeRenderers.ContainsKey(nodeName))
                graphLoader.nodeRenderers[nodeName].material = graphLoader.cyanMaterial;
        }

        // Spawn arrows between nodes
        
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 currentPos = path[i].transform.position;
            Vector3 nextPos = path[i + 1].transform.position;
            Vector3 midPoint = (currentPos + nextPos) / 2f;

            // Keep arrow at same height as nodes — no vertical offset
            Vector3 arrowPos = new Vector3(midPoint.x, currentPos.y, midPoint.z);

            // Force next position to same Y so LookAt stays horizontal
            Vector3 nextPosFlat = new Vector3(nextPos.x, currentPos.y, nextPos.z);

            GameObject arrow = Instantiate(arrowPrefab, arrowPos, Quaternion.identity);
            arrow.transform.LookAt(nextPosFlat);
            arrow.transform.rotation *= Quaternion.Euler(90f, 0f, 0f);

            // Scale arrow down
            arrow.transform.localScale = new Vector3(0.3f, 0.2f, 0.3f);

            spawnedObjects.Add(arrow);
        }
    }
}


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Arrowspawner : MonoBehaviour
// {
//     public GameObject arrowPrefab;
//     public PathFinder pathFinder;
//     public GraphLoader graphLoader;

//     [Header("Node Visuals")]
//     public float nodeRadius = 0.03f;
//     public Color nodeColor = Color.cyan;

//     private List<GameObject> spawnedObjects = new List<GameObject>();

//     void Start()
//     {
//         StartCoroutine(SpawnAfterLoad());
//     }

//     IEnumerator SpawnAfterLoad()
//     {
//         yield return new WaitForSeconds(3f);
//         SpawnPath();
//     }

//     public void RespawnPath()
//     {
//         foreach (GameObject obj in spawnedObjects)
//             Destroy(obj);
//         spawnedObjects.Clear();

//         SpawnPath();
//     }

//     void SpawnPath()
//     {
//         List<Node> path = pathFinder.FindPath();

//         if (path.Count == 0)
//         {
//             Debug.LogWarning("No path found!");
//             return;
//         }

//         Debug.Log("Path found with " + path.Count + " nodes!");

//         // Reset all nodes to yellow
//         foreach (var key in graphLoader.nodeRenderers.Keys)
//             graphLoader.nodeRenderers[key].material.color = Color.yellow;

//         // Highlight path nodes cyan
//         foreach (Node node in path)
//         {
//             string nodeName = node.gameObject.name;
//             if (graphLoader.nodeRenderers.ContainsKey(nodeName))
//                 graphLoader.nodeRenderers[nodeName].material.color = Color.cyan;
//         }

//         // Spawn arrows between nodes
//         for (int i = 0; i < path.Count - 1; i++)
//         {
//             Vector3 currentPos = path[i].transform.position;
//             Vector3 nextPos = path[i + 1].transform.position;
//             Vector3 midPoint = (currentPos + nextPos) / 2f;
//             Vector3 arrowPos = midPoint + Vector3.up * 0.05f;

//             GameObject arrow = Instantiate(arrowPrefab, arrowPos, Quaternion.identity);
//             arrow.transform.LookAt(nextPos);
//             arrow.transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
//             spawnedObjects.Add(arrow);
//         }
//     }
// }



// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Arrowspawner : MonoBehaviour
// {
//     public GameObject arrowPrefab;
//     public PathFinder pathFinder;

//     [Header("Node Visuals")]
//     public float nodeRadius = 0.05f;
//     public Color nodeColor = Color.cyan;

//     void Start()
//     {
//         StartCoroutine(SpawnAfterLoad());
//     }

//     IEnumerator SpawnAfterLoad()
//     {
//         // Wait for GraphLoader to finish
//         yield return new WaitForSeconds(3f);

//         List<Node> path = pathFinder.FindPath();

//         if (path.Count == 0)
//         {
//             Debug.LogWarning("No path found!");
//             yield break;
//         }

//         Debug.Log("Path found with " + path.Count + " nodes!");

//         for (int i = 0; i < path.Count - 1; i++)
//         {
//             Vector3 currentPos = path[i].transform.position;
//             Vector3 nextPos = path[i + 1].transform.position;

//             Vector3 midPoint = (currentPos + nextPos) / 2f;

//             GameObject arrow = Instantiate(arrowPrefab, midPoint, Quaternion.identity);
//             arrow.transform.LookAt(nextPos);
//             arrow.transform.rotation *= Quaternion.Euler(90f, 0f, 0f);

//             SpawnNodeSphere(path[i]);
//         }

//         SpawnNodeSphere(path[path.Count - 1]);
//     }

//     void SpawnNodeSphere(Node node)
//     {
//         GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//         sphere.transform.position = node.transform.position + Vector3.up * 0.01f;
//         sphere.transform.localScale = Vector3.one * (nodeRadius * 1.5f);

//         Renderer r = sphere.GetComponent<Renderer>();
//         r.material = new Material(Shader.Find("Unlit/Color"));
//         r.material.color = nodeColor;
//     }
// }
