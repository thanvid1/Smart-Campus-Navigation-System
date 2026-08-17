using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    void Awake()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 2f);
        cube.transform.localScale = Vector3.one * 0.5f;

        // Use URP shader instead
        Renderer r = cube.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = Color.red;
        r.material = mat;
    }
}