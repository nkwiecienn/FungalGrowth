using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.IO;

public class MapEditor : MonoBehaviour 
{
    [SerializeField] private Tilemap Tilemap;
    [SerializeField] private GameObject hexagonPrefab;
    
    public Dictionary<Vector3Int, Cell> grid;
    private Dictionary<Vector3Int, GameObject> cellVisuals;

    private void Awake()
    {    
        grid = new Dictionary<Vector3Int, Cell>();
        cellVisuals  = new Dictionary<Vector3Int, GameObject>();
    }

    void Start()
    { 
        Initialize();
    }

    void Update()
    {
    }


    private void Initialize()
{
    string filePath = "Assets/Data/initParams.json";
        if (!System.IO.File.Exists(filePath)) {
            return;
        }

        string json = System.IO.File.ReadAllText(filePath);
        SerializableInitParams initParams = JsonUtility.FromJson<SerializableInitParams>(json);

    int gridWidth = 18;
    int gridHeight = 36;

    for (int x = -gridWidth; x <= gridWidth; x++)
    {
        for (int y = -gridHeight; y <= gridHeight; y++)
        {
            Vector3Int position = new Vector3Int(x, y, 0);

            Cell newCell = new GameObject($"Cell_{position}").AddComponent<Cell>();

            newCell.EnvironmentNutrients = Random.Range(initParams.minEnvSubstrate, initParams.maxEnvSubstrate + 1);
            newCell.isTip = false;

            for (int i = 0; i < newCell.Mycelium.Length; i++)
            {
                newCell.Mycelium[i] = new Cell.MyceliumConnection
                {
                    isActive = false,
                    nutrients = 0f
                };
            }

            grid[position] = newCell;
        }
    }

    int totalCells = (gridWidth * 2 + 1) * (gridHeight * 2 + 1);
    int richSpotCount = Mathf.FloorToInt(totalCells * initParams.richSpotsNumber / 100);

    for (int i = 0; i < richSpotCount; i++)
    {
        int centerX = Random.Range(-gridWidth, gridWidth + 1);
        int centerY = Random.Range(-gridHeight, gridHeight + 1);
        Vector3Int centerPosition = new Vector3Int(centerX, centerY, 0);

        int radius = Mathf.FloorToInt(gridWidth * initParams.richSpotsSize);

        foreach (var kvp in grid)
        {
            Vector3Int pos = kvp.Key;
            float distance = Vector3Int.Distance(centerPosition, pos);

            if (distance <= radius)
            {
                Cell cell = kvp.Value;
                float additionalNutrients = 3 * Random.Range(
                    0,
                    initParams.maxEnvSubstrate * initParams.richSpotsDensity
                );

                float oldNutrients = cell.EnvironmentNutrients;
                cell.EnvironmentNutrients = Mathf.Clamp(
                    cell.EnvironmentNutrients + additionalNutrients,
                    0,
                    100
                );
            }
        }
    }
    foreach (var kvp in grid)
    {
        Vector3Int pos = kvp.Key;
        Cell cell = kvp.Value;

        UpdateOrCreateVisual(pos, cell);
    }
}

    public void UpdateOrCreateVisual(Vector3Int pos, Cell cell)
    {
        Vector3 worldPosition = Tilemap.CellToWorld(pos);
        if (!cellVisuals.TryGetValue(pos, out GameObject hex))
        {
            hex = Instantiate(hexagonPrefab);
            cellVisuals[pos] = hex;
        }

        hex.transform.position = worldPosition;

        UpdateHexagonAppearance(hex, cell, worldPosition);
    }

    private void UpdateHexagonAppearance(GameObject hexagon, Cell cell, Vector3 worldPos)
    {
        Color baseColor = Color.Lerp(Color.red, Color.blue, cell.EnvironmentNutrients / 100f);
        baseColor = Color.Lerp(baseColor, Color.white, 0.3f);

        var renderer = hexagon.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = baseColor;

        LineRenderer[] lines = hexagon.GetComponentsInChildren<LineRenderer>();
        if (lines.Length < 6) return;

        float radius = 0.5f * Mathf.Sqrt(3) / 2;
        Vector3[] directions = new Vector3[]
        {
            new Vector3(-radius * Mathf.Sqrt(3) / 2,  radius / 2, 0),
            new Vector3(0,                           radius,      0),
            new Vector3( radius * Mathf.Sqrt(3) / 2, radius / 2,  0),
            new Vector3( radius * Mathf.Sqrt(3) / 2, -radius / 2, 0),
            new Vector3(0,                          -radius,      0),
            new Vector3(-radius * Mathf.Sqrt(3) / 2, -radius / 2, 0),
        };

        for (int i = 0; i < 6; i++)
        {
            LineRenderer lr = lines[i];
            if (!cell.Mycelium[i].isActive)
            {
                lr.positionCount = 0;
                continue;
            }

            lr.positionCount = 2;
            lr.SetPosition(0, worldPos);
            lr.SetPosition(1, worldPos + directions[i]);

            float ratio = cell.Mycelium[i].nutrients / 100f;
            Color lineColor = Color.Lerp(Color.red, Color.blue, ratio);
            lr.startColor = lineColor;
            lr.endColor   = lineColor;

            lr.startWidth = 0.05f;
            lr.endWidth   = 0.05f;
        }
    }

    public void NutrientsUp(Vector3Int pos) {
        pos.z = 0;

        if (grid.TryGetValue(pos, out Cell clickedCell))
        {
            clickedCell.EnvironmentNutrients += 10;
            UpdateOrCreateVisual(pos, clickedCell);
        }
    }

    public Cell GetCell(Vector3Int pos) {
        pos.z = 0;
        return grid[pos];
    }


    public void SaveGridToJson()
    {
        string filePath = "Assets/Data/cellPattern.json";
        var gridData = new GridData();

        foreach (var kvp in grid)
        {
            Vector3Int position = kvp.Key;
            Cell cell = kvp.Value;

            gridData.Cells.Add(GridData.CellData.FromCell(position, cell));
        }

        string json = JsonUtility.ToJson(gridData, true);
        System.IO.File.WriteAllText(filePath, json);
    }
}
