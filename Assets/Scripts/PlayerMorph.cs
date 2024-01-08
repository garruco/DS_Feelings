using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Noise;
using Terrain;

namespace Player
{
public class PlayerMorph : MonoBehaviour
{
    public NoiseManager noiseManager;
    public TerrainManager terrainManager;
    public Material playerMaterial;
    void Start()
    {
        noiseManager = GameObject.FindGameObjectWithTag("NoiseManager").GetComponent<NoiseManager>();
        terrainManager = GameObject.FindGameObjectWithTag("TerrainManager").GetComponent<TerrainManager>();
    }

    void Update()
    {
        float arousalRaw = noiseManager.GetArousalRaw();
        float arousalEdited = noiseManager.GetArousalEdited();

        float valenceRaw = noiseManager.GetValenceRaw();
        float valenceEdited = noiseManager.GetValenceEdited();

        playerMaterial.SetColor("_Color", GetColor(valenceRaw, arousalRaw));
        playerMaterial.SetFloat("_Smoothness", GetSmoothness(arousalEdited));
        playerMaterial.SetFloat("_Metallic", GetMetallic(valenceEdited));
    }

    Color GetColor(float valence, float arousal)
    {
        float terrainHue = terrainManager.GetHue();

        float finalHue = (terrainHue + arousal * 10) / 360;

        float finalSaturation = (valence + 1) / 2 + 0.1f;
        finalSaturation = Mathf.Clamp(finalSaturation, 0, 1);

        float finalValue = (arousal + 1) / 2 + 0.3f;
        finalSaturation = Mathf.Clamp(finalSaturation, 0, 1);

        return Color.HSVToRGB(finalHue, finalSaturation, finalValue);
    }

    float GetSmoothness(float arousal)
    {
        float smoothness = (arousal + 1) / 2 + 0.2f;
        smoothness = Mathf.Clamp(smoothness, 0, 1);

        return smoothness;
    }

    float GetMetallic(float valence)
    {
        float metallic = (valence + 1) / 2;
        metallic = Mathf.Clamp(metallic, 0, 1);

        return metallic;
    }
}
}
