using System.Collections.Generic;
using UnityEngine;

public class GridInitializer
{
    public void CreateGrid(Dictionary<Vector3Int, Cell> grid)
    {
        int gridSize = 20;
        int halfGrid = gridSize / 2;

        for (int x = -10; x <= 10; x++)
        {
            for (int y = -20; y <= 20; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                Cell newCell = new GameObject("Cell").AddComponent<Cell>();
                newCell.EnvironmentNutrients = Random.Range(0, 100); 
                newCell.isTip = false;
                
                for (int i = 0; i < newCell.mycelium.Length; i++) {
                    newCell.mycelium[i] = new Cell.MyceliumConnection { isActive = false, nutrients = 0f };
                }

                grid[position] = newCell;
            }
        }
    }
}
