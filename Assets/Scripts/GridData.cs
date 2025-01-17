using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridData
{
    public List<CellData> Cells = new List<CellData>();

    [Serializable]
    public class CellData
    {
        public int X;
        public int Y;
        public float EnvironmentNutrients;
        public bool IsTip;
        public List<MyceliumConnectionData> Mycelium = new List<MyceliumConnectionData>();

        [Serializable]
        public class MyceliumConnectionData
        {
            public bool IsActive;
            public float Nutrients;
        }

        public static CellData FromCell(Vector3Int position, Cell cell)
        {
            var cellData = new CellData
            {
                X = position.x,
                Y = position.y,
                EnvironmentNutrients = cell.EnvironmentNutrients,
                IsTip = cell.isTip
            };

            foreach (var myceliumConnection in cell.Mycelium)
            {
                cellData.Mycelium.Add(new MyceliumConnectionData
                {
                    IsActive = myceliumConnection.isActive,
                    Nutrients = myceliumConnection.nutrients
                });
            }

            return cellData;
        }
    }
}
