using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Serializable] 
    public class MyceliumConnection {
        public bool isActive = false;
        public float nutrients = 0;
        public float nutrientsChange = 0;

        public float Nutrients {
            get => nutrients;
            set => nutrients = Mathf.Clamp(value, 0, 100);
        }
    }

    [SerializeField] public MyceliumConnection[] mycelium = new MyceliumConnection[6];
    [SerializeField] public float environmentNutrients;
    [SerializeField] public bool isTip;

    public MyceliumConnection[] Mycelium => mycelium;
    public float EnvironmentNutrients {
        get => environmentNutrients;
        set => environmentNutrients = Mathf.Clamp(value, 0, 100);
    }

    private void Awake()
    {
        InitializeMycelium();
    }

    public void InitializeMycelium()
    {
        for (int i = 0; i < mycelium.Length; i++)
        {
            if (mycelium[i] == null)
            {
                mycelium[i] = new MyceliumConnection();
            }
        }
    }

    static Vector3Int[] neighborOffsets0 = new Vector3Int[] {
        new Vector3Int(0, -1, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0),
        new Vector3Int(-1, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(-1, -1, 0)
    };

    static Vector3Int[] neighborOffsets1 = new Vector3Int[] {
        new Vector3Int(1, -1, 0), new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0)
    };

    public static List<Vector3Int> getNeighbors(Vector3Int position) {
        var neighbors = new List<Vector3Int>();

        Vector3Int[] neighborOffsets = position.y % 2 == 0 ? neighborOffsets0 : neighborOffsets1;
        foreach (Vector3Int offset in neighborOffsets) {
            neighbors.Add(position + offset);
        }

        return neighbors;
    }

    public bool isEmpty() {
        for(int i = 0; i < 6; i++) {
            if(Mycelium[i].isActive)
                return false;
        }

        return true;
    }

    public int countMycelium() {
        int count = 0;
        for(int i = 0; i < 6; i++) {
            if(Mycelium[i].isActive)
                count++;
        }

        return count;
    }

    public void externalSubstrate(float c2)
    {
        int active = countMycelium();
        if (active <= 0) return;

        float substrate = c2 * EnvironmentNutrients;
        EnvironmentNutrients -= substrate;

        float portion = substrate / active;
        for (int i = 0; i < 6; i++)
        {
            if (Mycelium[i].isActive)
            {
                Mycelium[i].Nutrients += portion;
            }
        }
    }

    public void diffusionCalculate(float d1, List<Cell> neighbors)
    {
        for (int i = 0; i < 6; i++)
        {
            if (!Mycelium[i].isActive) continue;

            Cell neighborCell = (i < neighbors.Count) ? neighbors[i] : null;
            if (neighborCell == null) continue;

            float neighborNutrients = neighborCell.Mycelium[(i + 3) % 6].nutrients;

            float difference = (Mycelium[i].nutrients - neighborNutrients) / 2f;

            Mycelium[i].nutrientsChange = d1 * difference;
        }
    }

    public void diffusionAdd() {
        foreach(MyceliumConnection m in Mycelium) {
            if(!m.isActive) continue;
            m.nutrients += m.nutrientsChange;
            m.nutrientsChange = 0f;
        }
    }

    public int getTipPosition() {
        for(int i = 0; i < 6; i++) {
            if(Mycelium[i].isActive)
                return i;
        }

        return -1;
    }

    public float translocation(float d2) {
        float nutrientsChange = 0;

        foreach(MyceliumConnection m in Mycelium) {
            if(m.isActive) {
                float change = d2 * m.nutrients;
                nutrientsChange += change;
                m.nutrients -= change;
            }
        }

        return nutrientsChange;
    }

    public float[] getPropability(float Dp, float v) {
        var propability = new float[6];

        int tipPosition = getTipPosition();

        propability[(tipPosition + 2) % 6] = (Dp * Mycelium[tipPosition].nutrients) / 400;
        propability[(tipPosition + 3) % 6] = (Dp * Mycelium[tipPosition].nutrients * (1 + v)) / 400;  
        propability[(tipPosition + 4) % 6] = (Dp * Mycelium[tipPosition].nutrients) / 400;

        return propability;
    }

    public float getBranchingPropability(float b) {
        int tipPosition = getTipPosition();

        return (b * Mycelium[tipPosition].nutrients);
    }

    public void grow(int direction, float nutrients) {
        int tip = getTipPosition();

        if(tip != -1) {
            if (Mycelium[tip].nutrients < nutrients) return;
            Mycelium[tip].Nutrients = Mycelium[tip].nutrients - nutrients;
        }

        Mycelium[direction].isActive = true;
        Mycelium[direction].Nutrients = nutrients;

        if(countMycelium() == 1) {
            isTip = true;
        } else {
            isTip = false;
        }
    }
}
