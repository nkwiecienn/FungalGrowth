using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Params : MonoBehaviour
{
    
    SerializableParams serializableParams = new SerializableParams();
    SerializableInitParams serializableInitParams = new SerializableInitParams();

    public void SetGrowth(float growth) {
        serializableParams.growth = growth;
    }

    public void SetGrowthForward(float growthForward) {
        serializableParams.growthForward = growthForward;
    }

    public void SetBranching(float branching) {
        serializableParams.branching = branching;
    }

    public void SetDiffusion(float diffusion) {
        serializableParams.diffusion = diffusion;
    }

    public void SetCellMovement(float cellMovement) {
        serializableParams.cellMovement = cellMovement;
    }

    public void SetEnvironmentSubstrate(float environmentSubstrate) {
        serializableParams.environmentSubstrate = environmentSubstrate;
    }

    public void SetCellCreationSubstrate(float cellCreationSubstrate) {
        serializableParams.cellCreationSubstrate = cellCreationSubstrate;
    }

    public void SetMinEnvSubstrate(float minEnvSubstrate) {
        serializableInitParams.minEnvSubstrate = minEnvSubstrate;
    }

    public void SetMaxEnvSubstrate(float maxEnvSubstrate) {
        serializableInitParams.maxEnvSubstrate = maxEnvSubstrate;
    }

    public void SetRichSpotsNumber(float richSpotsNumber) {
        serializableInitParams.richSpotsNumber = richSpotsNumber;
    }

    public void SetRichSpotsSize(float richSpotsSize) {
        serializableInitParams.richSpotsSize = richSpotsSize;
    }

    public void SetRichSpotsDensity(float richSpotsDensity) {
        serializableInitParams.richSpotsDensity = richSpotsDensity;
    }

    public void Save() {
        string filePathParams = "Assets/Data/params.json";
        string filePathInitParams = "Assets/Data/initParams.json";

        string jsonParams = JsonUtility.ToJson(serializableParams, true);
        System.IO.File.WriteAllText(filePathParams, jsonParams);

        string jsonInitParams = JsonUtility.ToJson(serializableInitParams, true);
        System.IO.File.WriteAllText(filePathInitParams, jsonInitParams);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
