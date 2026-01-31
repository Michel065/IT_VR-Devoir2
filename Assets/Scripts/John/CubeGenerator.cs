using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    public GameObject cubePrefab;

    public GameObject CreateCube(Vector3 localPos, Vector3 localSize, string name = "Cube")
    {
        if (!cubePrefab) return null;

        GameObject cube = Instantiate(cubePrefab, transform);
        cube.name = name;
        cube.transform.localPosition = localPos;
        cube.transform.localRotation = Quaternion.identity;
        cube.transform.localScale = localSize;
        return cube;
    }
}
