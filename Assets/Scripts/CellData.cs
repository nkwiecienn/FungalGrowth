using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CellData
{
    public Vector3Int Position;
    public float EnvironmentNutrients;
    public bool IsTip;
    public MyceliumConnectionData[] Mycelium;

    [Serializable]
    public class MyceliumConnectionData
    {
        public bool IsActive;
        public float Nutrients;
        public float NutrientsChange;
    } 

    public static CellData FromCell(Vector3Int position, Cell cell)
    {
        return new CellData
        {
            Position = position,
            EnvironmentNutrients = cell.EnvironmentNutrients,
            IsTip = cell.isTip,
            Mycelium = Array.ConvertAll(cell.Mycelium, m => new MyceliumConnectionData
            {
                IsActive = m.isActive,
                Nutrients = m.nutrients,
                NutrientsChange = m.nutrientsChange
            })
        };
    }

    public void ApplyToCell(Cell cell)
    {
        cell.EnvironmentNutrients = EnvironmentNutrients;
        cell.isTip = IsTip;

        for (int i = 0; i < Mycelium.Length; i++)
        {
            cell.Mycelium[i].isActive = Mycelium[i].IsActive;
            cell.Mycelium[i].nutrients = Mycelium[i].Nutrients;
            cell.Mycelium[i].nutrientsChange = Mycelium[i].NutrientsChange;
        }
    }
}
