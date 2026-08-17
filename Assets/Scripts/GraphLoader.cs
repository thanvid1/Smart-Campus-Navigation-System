using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class NodeData
{
    public string id;
    public float lat;
    public float lon;
    public int floor;
}

[System.Serializable]
public class EdgeData
{
    public string from;
    public string to;
    public float weight;
}

[System.Serializable]
public class GraphData
{
    public List<NodeData> nodes;
    public List<EdgeData> edges;
}

public class GraphLoader : MonoBehaviour
{
    public float yHeight = 0f;
    public float mapScale = 0.15f;
    public float spawnDistance = 1.5f;
    public float nodeRadius = 0.05f;
    public Color nodeColor = Color.yellow;

    public Material yellowMaterial;
    public Material cyanMaterial;

    [HideInInspector]
    public Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();

    [HideInInspector]
    public Dictionary<string, Renderer> nodeRenderers = new Dictionary<string, Renderer>();

    void Awake()
    {
        Debug.Log("GraphLoader Awake called!");
        StartCoroutine(LoadAfterCamera());
    }

    IEnumerator LoadAfterCamera()
    {
        yield return new WaitForSeconds(2f);

        // Request GPS permission explicitly on Android
        #if UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(
            UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(
                UnityEngine.Android.Permission.FineLocation);
            yield return new WaitForSeconds(2f); // Wait for user to respond
        }
        #endif

        // Wait for camera
        Camera cam = null;
        while (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
                yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Camera found: " + cam.name);

        // Start GPS
        Input.location.Start(1f, 0.1f);

        // Wait for GPS to initialize (max 20 seconds)
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1f);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("GPS failed! Using fallback placement.");
            yield return StartCoroutine(LoadGraphFromFile(cam, null));
            yield break;
        }

        Debug.Log("GPS started! Lat: " + Input.location.lastData.latitude
            + " Lon: " + Input.location.lastData.longitude);

        yield return StartCoroutine(LoadGraphFromFile(cam, Input.location.lastData));
    }

    IEnumerator LoadGraphFromFile(Camera cam, LocationInfo? gpsData)
    {
        string filePath = System.IO.Path.Combine(
            Application.streamingAssetsPath, "graph.json");

        UnityWebRequest request = UnityWebRequest.Get(filePath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load graph.json: " + request.error);
            yield break;
        }

        Debug.Log("graph.json loaded successfully!");
        string json = request.downloadHandler.text;
        LoadGraph(json, cam, gpsData);
    }

    void LoadGraph(string json, Camera cam, LocationInfo? gpsData)
    {
        GraphData graph = JsonUtility.FromJson<GraphData>(json);

        Vector3 camPos = cam.transform.position;
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 origin = camPos + camForward * spawnDistance;
        origin.y = camPos.y + yHeight;

        // Reference GPS point — either from GPS or college center
        float refLat = gpsData.HasValue ? gpsData.Value.latitude : 13.254694f;
        float refLon = gpsData.HasValue ? gpsData.Value.longitude : 74.784972f;

        foreach (NodeData nd in graph.nodes)
        {
            Vector3 nodePos;

            // Convert GPS lat/lon offset to meters
            float deltaLat = (nd.lat - refLat) * 111000f;
            float deltaLon = (nd.lon - refLon) * 111000f
                * Mathf.Cos(refLat * Mathf.Deg2Rad);

            // Scale down for AR view
            nodePos = origin
                + camRight * (deltaLon * mapScale)
                + camForward * (deltaLat * mapScale);
            nodePos.y = origin.y;

            GameObject go = new GameObject(nd.id);
            go.transform.position = nodePos;
            Node node = go.AddComponent<Node>();
            nodeMap[nd.id] = node;

            SpawnNodeSphere(nodePos, nd.id);
            SpawnLabel(nd.id, nodePos);
        }

        // Connect edges
        foreach (EdgeData ed in graph.edges)
        {
            if (nodeMap.ContainsKey(ed.from) && nodeMap.ContainsKey(ed.to))
            {
                nodeMap[ed.from].neighbors.Add(nodeMap[ed.to]);
            }
        }

        Debug.Log("Graph loaded! Total nodes: " + nodeMap.Count);
    }

    void SpawnNodeSphere(Vector3 position, string nodeId)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * nodeRadius;

        Renderer r = sphere.GetComponent<Renderer>();
        r.material = yellowMaterial;
        nodeRenderers[nodeId] = r;
    }

    void SpawnLabel(string text, Vector3 position)
    {
        var textObj = new GameObject(text + "_Label");
        textObj.transform.position = position + Vector3.up * (nodeRadius + 0.05f);
        var tm = textObj.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 12;
        tm.color = Color.white;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.characterSize = 0.02f;
    }
}




// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Networking;

// [System.Serializable]
// public class NodeData
// {
//     public string id;
//     public float x;
//     public float y;
//     public int floor;
// }

// [System.Serializable]
// public class EdgeData
// {
//     public string from;
//     public string to;
//     public float weight;
// }

// [System.Serializable]
// public class GraphData
// {
//     public List<NodeData> nodes;
//     public List<EdgeData> edges;
// }

// public class GraphLoader : MonoBehaviour
// {
//     public float yHeight = 0f;
//     public float mapScale = 0.15f;
//     public float spawnDistance = 1.5f;
//     public float nodeRadius = 0.05f;
//     public Color nodeColor = Color.yellow;

//     public Material yellowMaterial;
//     public Material cyanMaterial;

//     [HideInInspector]
//     public Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();

//     [HideInInspector]
//     public Dictionary<string, Renderer> nodeRenderers = new Dictionary<string, Renderer>();

//     void Awake()
//     {
//         Debug.Log("GraphLoader Awake called!");
//         StartCoroutine(LoadAfterCamera());
//     }

//     IEnumerator LoadAfterCamera()
//     {
//         yield return new WaitForSeconds(2f);

//         Camera cam = null;
//         while (cam == null)
//         {
//             cam = Camera.main;
//             if (cam == null)
//                 yield return new WaitForSeconds(0.5f);
//         }

//         Debug.Log("Camera found: " + cam.name);

//         string filePath = System.IO.Path.Combine(
//             Application.streamingAssetsPath, "graph.json");

//         UnityWebRequest request = UnityWebRequest.Get(filePath);
//         yield return request.SendWebRequest();

//         if (request.result != UnityWebRequest.Result.Success)
//         {
//             Debug.LogError("Failed to load graph.json: " + request.error);
//             yield break;
//         }

//         Debug.Log("graph.json loaded successfully!");
//         string json = request.downloadHandler.text;
//         LoadGraph(json, cam);
//     }

//     void LoadGraph(string json, Camera cam)
//     {
//         GraphData graph = JsonUtility.FromJson<GraphData>(json);

//         Vector3 camPos = cam.transform.position;
//         Vector3 camForward = cam.transform.forward;
//         camForward.y = 0f;
//         camForward.Normalize();

//         Vector3 camRight = cam.transform.right;
//         camRight.y = 0f;
//         camRight.Normalize();

//         Vector3 origin = camPos + camForward * spawnDistance;
//         origin.y = camPos.y + yHeight;

//         float centerX = 0f, centerY = 0f;
//         foreach (NodeData nd in graph.nodes)
//         {
//             centerX += nd.x;
//             centerY += nd.y;
//         }
//         centerX /= graph.nodes.Count;
//         centerY /= graph.nodes.Count;

//         foreach (NodeData nd in graph.nodes)
//         {
//             float offsetX = (nd.x - centerX) * mapScale;
//             float offsetZ = (nd.y - centerY) * mapScale;

//             Vector3 nodePos = origin
//                 + camRight * offsetX
//                 + camForward * offsetZ;
//             nodePos.y = origin.y;

//             GameObject go = new GameObject(nd.id);
//             go.transform.position = nodePos;
//             Node node = go.AddComponent<Node>();
//             nodeMap[nd.id] = node;

//             SpawnNodeSphere(nodePos, nd.id);
//             SpawnLabel(nd.id, nodePos);
//         }

//         foreach (EdgeData ed in graph.edges)
//         {
//             if (nodeMap.ContainsKey(ed.from) && nodeMap.ContainsKey(ed.to))
//             {
//                 nodeMap[ed.from].neighbors.Add(nodeMap[ed.to]);
//             }
//         }

//         Debug.Log("Graph loaded! Total nodes: " + nodeMap.Count);
//     }

//     void SpawnNodeSphere(Vector3 position, string nodeId)
//     {
//         GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//         sphere.transform.position = position;
//         sphere.transform.localScale = Vector3.one * nodeRadius;

//         Renderer r = sphere.GetComponent<Renderer>();
//         r.material = yellowMaterial;
//         nodeRenderers[nodeId] = r;
//     }

//     void SpawnLabel(string text, Vector3 position)
//     {
//         var textObj = new GameObject(text + "_Label");
//         textObj.transform.position = position + Vector3.up * (nodeRadius + 0.05f);
//         var tm = textObj.AddComponent<TextMesh>();
//         tm.text = text;
//         tm.fontSize = 12;
//         tm.color = Color.white;
//         tm.anchor = TextAnchor.MiddleCenter;
//         tm.characterSize = 0.02f;
//     }
// }

