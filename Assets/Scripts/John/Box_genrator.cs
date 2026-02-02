using UnityEngine;

public class Box_generator : MonoBehaviour
{
    [Header("Creation")]
    public bool Murs = true;
    public bool Sol = true;
    public bool Plafond = true;

    [Header("Param salle")]
    public float roomLength = 8f;
    public float roomWidth = 6f;
    public float roomHeight = 3f;
    public float thickness = 0.2f;

    [Header("Prefab")]
    public GameObject cubePrefab;

    [Header("Organisation")]
    public bool ensemble = false;

    void Start()
    {
        if (!cubePrefab) return;

        GameObject root = new GameObject("Box");
        root.transform.position = transform.position;
        root.transform.rotation = transform.rotation;

        if (ensemble)
        {
            CubeGenerator genBox = root.AddComponent<CubeGenerator>();
            genBox.cubePrefab = cubePrefab;

            GenerateAll(genBox);
        }
        else
        {
            if (Murs)
            {
                GameObject wallsObj = new GameObject("Murs");
                wallsObj.transform.SetParent(root.transform, false);
                CubeGenerator genWalls = wallsObj.AddComponent<CubeGenerator>();
                genWalls.cubePrefab = cubePrefab;
                GenerateWalls(genWalls);
            }

            if (Sol)
            {
                GameObject floorObj = new GameObject("Sol");
                floorObj.transform.SetParent(root.transform, false);
                CubeGenerator genFloor = floorObj.AddComponent<CubeGenerator>();
                genFloor.cubePrefab = cubePrefab;
                GenerateFloor(genFloor);
            }

            if (Plafond)
            {
                GameObject ceilObj = new GameObject("Plafond");
                ceilObj.transform.SetParent(root.transform, false);
                CubeGenerator genCeil = ceilObj.AddComponent<CubeGenerator>();
                genCeil.cubePrefab = cubePrefab;
                GenerateCeiling(genCeil);
            }
        }
    }

    void GenerateAll(CubeGenerator gen)
    {
        if (Murs) GenerateWalls(gen);
        if (Sol) GenerateFloor(gen);
        if (Plafond) GenerateCeiling(gen);
    }

    void GenerateWalls(CubeGenerator gen)
    {
        float L = roomLength;
        float W = roomWidth;
        float H = roomHeight;
        float t = thickness;

        float yCenter = H * 0.5f;

        // Mur +Z
        gen.CreateCube(
            new Vector3(0f, yCenter, (W * 0.5f) + (t * 0.5f)),
            new Vector3(L + 2f * t, H, t),
            "Wall_+Z"
        );

        // Mur -Z
        gen.CreateCube(
            new Vector3(0f, yCenter, -(W * 0.5f) - (t * 0.5f)),
            new Vector3(L + 2f * t, H, t),
            "Wall_-Z"
        );

        // Mur +X
        gen.CreateCube(
            new Vector3((L * 0.5f) + (t * 0.5f), yCenter, 0f),
            new Vector3(t, H, W),
            "Wall_+X"
        );

        // Mur -X
        gen.CreateCube(
            new Vector3(-(L * 0.5f) - (t * 0.5f), yCenter, 0f),
            new Vector3(t, H, W),
            "Wall_-X"
        );
    }

    void GenerateFloor(CubeGenerator gen)
    {
        float L = roomLength;
        float W = roomWidth;
        float t = thickness;

        gen.CreateCube(
            new Vector3(0f, -(t * 0.5f), 0f),
            new Vector3(L, t, W),
            "Floor"
        );
    }

    void GenerateCeiling(CubeGenerator gen)
    {
        float L = roomLength+1;
        float W = roomWidth+1;
        float H = roomHeight;
        float t = thickness;

        gen.CreateCube(
            new Vector3(0f, H + (t * 0.5f), 0f),
            new Vector3(L, t, W),
            "Ceiling"
        );
    }
}
