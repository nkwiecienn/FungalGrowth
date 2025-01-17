using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellBoard : MonoBehaviour {
    [SerializeField] private Dictionary<Vector3Int, Cell> currentState;
    private Dictionary<Vector3Int, GameObject> cellVisuals;
    [SerializeField] private Tilemap Tilemap;
    [SerializeField] private GameObject hexagonPrefab;
    [SerializeField] private float updateInterval = 5f;

    private HashSet<Vector3Int> cellsToCheck;
    private HashSet<Vector3Int> tips;
    private HashSet<Vector3Int> nextTips;
    private HashSet<Vector3Int> nextCellsToCheck;

    [SerializeField] private float c1 = 10f;
    [SerializeField] private float c2 = 0.2f;
    [SerializeField] private float d1 = 0.1f;
    [SerializeField] private float d2 = 0.5f;
    [SerializeField] private float Dp = 1f;
    [SerializeField] private float v = 0.5f;
    [SerializeField] private float b = 0.3f;
 
    private void Awake() {
        cellsToCheck = new HashSet<Vector3Int>();
        tips = new HashSet<Vector3Int>();
        nextTips = new HashSet<Vector3Int>();
        nextCellsToCheck = new HashSet<Vector3Int>();
        currentState = new Dictionary<Vector3Int, Cell>();
        cellVisuals  = new Dictionary<Vector3Int, GameObject>();
    }

    private void Start() {
        SetParams();
        SetPattern();
    }

    private void OnEnable() {
        StartCoroutine(Simulate());
    }

    private IEnumerator Simulate() {
    
        var interval = new WaitForSeconds(updateInterval);
        yield return interval;

        while(enabled) {
            UpdateState();
            yield return interval;
        }
    }

    private void UpdateState() {

        foreach (Vector3Int position in cellsToCheck) {
            if (!currentState.ContainsKey(position)) 
                continue;

            nextCellsToCheck.Add(position);

            currentState[position].externalSubstrate(c2);
        }

        foreach (Vector3Int position in cellsToCheck)
        {
            if (!currentState.ContainsKey(position)) 
                continue;
            List<Vector3Int> neighborPositions = Cell.getNeighbors(position);
            
            var neighborCells = new List<Cell>();
            foreach (var npos in neighborPositions)
            {
                currentState.TryGetValue(npos, out Cell neighCell);
                neighborCells.Add(neighCell);
            }
            currentState[position].diffusionCalculate(d1, neighborCells);
        }

        foreach (Vector3Int position in cellsToCheck)
        {
            if (!currentState.ContainsKey(position)) continue;
            currentState[position].diffusionAdd();
        }

        foreach (Vector3Int tipPosition in tips) {
            if (!currentState.ContainsKey(tipPosition)) continue;

            int tip = currentState[tipPosition].getTipPosition();
            List<Vector3Int> neighbors = Cell.getNeighbors(tipPosition);

            Vector3Int tipBeginning = neighbors[tip];
            if (currentState.ContainsKey(tipBeginning)) {
                var cellBeginning = currentState[tipBeginning];
                float nutrientsChange = cellBeginning.translocation(d2);

                currentState[tipPosition].Mycelium[tip].nutrients += nutrientsChange;
            }
        }

        foreach (Vector3Int tipPosition in tips)
        {
            if (!currentState.ContainsKey(tipPosition)) 
                continue;
            int tipDir = currentState[tipPosition].getTipPosition();
            if (tipDir < 0) continue;

            var probabilities = currentState[tipPosition].getPropability(Dp, v);
            float branching = currentState[tipPosition].getBranchingPropability(b);
            if (branching >= 50f)
            {
                int dir1 = (tipDir + 2) % 6;
                int dir2 = (tipDir + 4) % 6;
                currentState[tipPosition].grow(dir1, c1);
                currentState[tipPosition].grow(dir2, c1);
                GrowNeighbor(tipPosition, dir1);
                GrowNeighbor(tipPosition, dir2);
            }
            else
            {
                int growthDir = GetGrowthPosition(probabilities);

                if (growthDir == -1) continue;

                currentState[tipPosition].grow(growthDir, c1);
                GrowNeighbor(tipPosition, growthDir);
            }
        }

        foreach (Vector3Int changePos in nextCellsToCheck)
        {
            if (!currentState.ContainsKey(changePos)) continue;
            Vector3 worldPosition = Tilemap.CellToWorld(changePos);
            UpdateOrCreateVisual(changePos, currentState[changePos], worldPosition);

            if (currentState[changePos].countMycelium() == 1)
            {
                nextTips.Add(changePos);
            }    

        }

        var tempTips = tips;
        tips = nextTips;
        nextTips = tempTips;
        nextTips.Clear();

        var tempCells = cellsToCheck;
        cellsToCheck = nextCellsToCheck;
        nextCellsToCheck = tempCells;
        nextCellsToCheck.Clear();
    }

    private int GetGrowthPosition(float[] probabilities) {
        Random.InitState(System.DateTime.Now.Millisecond);
        float random = Random.Range(0f, 1f);
        for (int i = 0; i < 6; i++) {
            if (random < probabilities[i]) {
                return i;
            }
            random -= probabilities[i];
        }
        return -1;
    }

    private void GrowNeighbor(Vector3Int cellPosition, int direction) {

        Vector3Int neighborPosition = Cell.getNeighbors(cellPosition)[direction];

        if (!currentState.ContainsKey(neighborPosition)) {
            return;

        }

        currentState[neighborPosition].grow((direction + 3) % 6, c1);

        if (currentState[neighborPosition].isTip) {
            nextTips.Add(neighborPosition);
        }

        nextCellsToCheck.Add(neighborPosition);
    }

    public void SetParams() {
        string filePath = "Assets/Data/params.json";
        if (!System.IO.File.Exists(filePath)) {
            return;
        }

        string json = System.IO.File.ReadAllText(filePath);
        SerializableParams serializableParams = JsonUtility.FromJson<SerializableParams>(json);

        Dp = serializableParams.growth;
        v = serializableParams.growthForward;
        b = serializableParams.branching;
        d1 = serializableParams.diffusion;
        d2 = serializableParams.cellMovement;
        c2 = serializableParams.environmentSubstrate;
        c1 = serializableParams.cellCreationSubstrate;
    }

    public void SetPattern()
    {
        string filePath = "Assets/Data/cellPattern.json";
        if (!System.IO.File.Exists(filePath))
        {
            return;
        }

        string json = System.IO.File.ReadAllText(filePath);
        GridData gridData = JsonUtility.FromJson<GridData>(json);

        foreach (var cellVisual in cellVisuals.Values)
        {
            Destroy(cellVisual);
        }
        cellVisuals.Clear();
        currentState.Clear();
        cellsToCheck.Clear();
        tips.Clear();

        foreach (var cellData in gridData.Cells)
        {
            Vector3Int position = new Vector3Int(cellData.X, cellData.Y, 0);

            GameObject cellObject = new GameObject($"Cell_{position}");
            cellObject.transform.position = Tilemap.CellToWorld(position);
            Cell newCell = cellObject.AddComponent<Cell>();

            newCell.EnvironmentNutrients = cellData.EnvironmentNutrients;
            newCell.isTip = cellData.IsTip;

            for (int i = 0; i < cellData.Mycelium.Count; i++)
            {
                var myceliumData = cellData.Mycelium[i];
                newCell.Mycelium[i] = new Cell.MyceliumConnection
                {
                    isActive = myceliumData.IsActive,
                    nutrients = myceliumData.Nutrients
                };
            }

            currentState[position] = newCell;

            int cellCount = newCell.countMycelium();
            if (cellCount > 0)
            {
                cellsToCheck.Add(position);

                if (cellCount == 1)
                {
                    tips.Add(position);
                }
            }

            UpdateOrCreateVisual(position, newCell, Tilemap.CellToWorld(position));
        }
    }


    private void UpdateOrCreateVisual(Vector3Int pos, Cell cell, Vector3 worldPosition)
    {
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
        
        float radius = 0.5f * Mathf.Sqrt(3) / 2;
        Vector3[] directions = new Vector3[] {
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

    private void Clear() {
        Tilemap.ClearAllTiles();
        currentState.Clear();
        cellsToCheck.Clear();
    }
}