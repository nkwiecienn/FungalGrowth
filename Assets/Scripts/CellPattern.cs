using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Fungal Simulation/CellPattern")]
public class CellPattern : ScriptableObject
{
    [SerializeField]
    public List<CellData> cellDataList = new List<CellData>();

    public void AddCellData(Vector3Int position, Cell cell)
    {
        cellDataList.Add(CellData.FromCell(position, cell));
    }

    public void ChangeCellData(Vector3Int position, Cell cell)
    {
        foreach (var cellData in cellDataList)
        {
            if (cellData.Position == position)
            {
                cellData.EnvironmentNutrients = cell.EnvironmentNutrients;
                cellData.IsTip = cell.isTip;

                for (int i = 0; i < cell.Mycelium.Length; i++)
                {
                    cellData.Mycelium[i].IsActive = cell.Mycelium[i].isActive;
                    cellData.Mycelium[i].Nutrients = cell.Mycelium[i].nutrients;
                    cellData.Mycelium[i].NutrientsChange = cell.Mycelium[i].nutrientsChange;
                }
                return;
            }
        }
    }

    public void ToJson()
    {
        string output = JsonUtility.ToJson(this, true);
        File.WriteAllText("Assets/Data/cellPattern.json", output);
    }

    public void FromJson(GameObject cellPrefab, Transform parent)
    {
        string json = File.ReadAllText("Assets/Data/cellPattern.json");
        JsonUtility.FromJsonOverwrite(json, this);

        foreach (var cellData in cellDataList)
        {
            GameObject cellObject = Instantiate(cellPrefab, parent);
            cellObject.transform.position = cellData.Position;

            var cell = cellObject.GetComponent<Cell>();
            if (cell != null)
            {
                cellData.ApplyToCell(cell);
            }
        }
    }
}
