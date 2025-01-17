using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Properties : Singleton<Builder>
{
    [SerializeField] Builder builder;
    [SerializeField] private List<TMP_InputField> tmpInputFields;
    [SerializeField] private Toggle[] toggles;

    public void SetNutrientsMycellium0(string nutrients) {
        if(nutrients == "" || nutrients == null) {
            return;
        }
        builder.cell.Mycelium[0].nutrients = float.Parse(nutrients);
    }

    public void SetNutrientsMycellium1(string nutrients) {
        if(nutrients == "" || nutrients == null) {
            return;
        }
        builder.cell.Mycelium[1].nutrients = float.Parse(nutrients);
    }

    public void SetNutrientsMycellium2(string nutrients) {
        if(nutrients == "" || nutrients == null) {
            return;
        }
        builder.cell.Mycelium[2].nutrients = float.Parse(nutrients);
    }

    public void SetNutrientsMycellium3(string nutrients) {
        if(nutrients == "" || nutrients == null) {
            return;
        }
        builder.cell.Mycelium[3].nutrients = float.Parse(nutrients);
    }

    public void SetNutrientsMycellium4(string nutrients) {
        if(nutrients == "" || nutrients == null) {
            return;
        }
        builder.cell.Mycelium[4].nutrients = float.Parse(nutrients);
    }

    public void SetNutrientsMycellium5(string nutrients) {
        if(nutrients == "" || nutrients == null) {
            return;
        }
        builder.cell.Mycelium[5].nutrients = float.Parse(nutrients);
    }

    public void setActive0 (bool active) {
        if(!active) return;
        builder.cell.Mycelium[0].isActive = active;
    }

    public void setActive1 (bool active) {
        if(!active) return;
        builder.cell.Mycelium[1].isActive = active;
    }

    public void setActive2 (bool active) {
        if(!active) return;
        builder.cell.Mycelium[2].isActive = active; 
    }

    public void setActive3 (bool active) {
        if(!active) return;
        builder.cell.Mycelium[3].isActive = active;
    }

    public void setActive4 (bool active) {
        if(!active) return;
        builder.cell.Mycelium[4].isActive = active;
    }

    public void setActive5 (bool active) {
        if(!active) return;
        builder.cell.Mycelium[5].isActive = active;
    }

    public void isTip(bool tip) {
        builder.cell.isTip = tip;
    }

    public void Environment(string environment) {
        if(environment == "" || environment == null) {
            return;
        }
        builder.cell.environmentNutrients = float.Parse(environment);
    }

    public void Save() {
        for(int i = 0; i < 6; i++) {
            if(builder.cell.Mycelium[i].nutrients == 0) {
                builder.cell.Mycelium[i].isActive = false;
            } else {
                builder.cell.Mycelium[i].isActive = true;
            }
        }

        if(builder.cell.countMycelium() == 1) {
            builder.cell.isTip = true;
        } else {
            builder.cell.isTip = false;
        }

        Vector3Int pos = builder.CurrentGridPosition;
        pos.z = 0;

        builder.MapEditor.UpdateOrCreateVisual(pos, builder.cell);

        ClearInputFields();
        ResetToggles();
        builder.Panel.SetActive(false);
        builder.OnEnable();
    }

    private void ClearInputFields()
    {
        foreach (var tmpInputField in tmpInputFields)
        {
            if (tmpInputField != null)
            {
                tmpInputField.text = "";
            }
        }
    }

    private void ResetToggles()
    {
        foreach (var toggle in toggles)
        {
            if (toggle != null)
            {
                toggle.isOn = false;
            }
        }
    }
}
