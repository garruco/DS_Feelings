using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Noise;
using Artifact;
using Player;
using Terrain;

public class ProgressManager : MonoBehaviour
{
    [Header("Managers")]
    public NoiseManager noiseManager;
    public ArtifactManager artifactManager;
    public PlayerMovement playerMovement;
    public TerrainManager terrainManager;
    public AudioManager audioManager;

    [Header("Progession Variables")]
    public List<int> neededDiamondsPerLevel;
    public List<float> feelingRadiusPerLevel;
    private bool levelingUp = false;

    float currentCubeHeight;
    private int currentLevel = 0;

    void Awake()
    {
        SetDiamondProgression();
        SetNoiseMax();
    }

    void LateUpdate()
    {
        if (levelingUp) ComputeLevelingUp();
    }

    void OnEnable()
    {
        ArtifactManager.OnRelicCollection += LevelingUp;
    }


    void OnDisable()
    {
        ArtifactManager.OnRelicCollection -= LevelingUp;
    }

    void LevelingUp()
    {
        Debug.Log("LEVELING UP");
        float rippleEffectDuration = 6f;
        audioManager.PlaySound("LevelUp");
        levelingUp = true;
        //playerMovement.DisableMovement();
        terrainManager.StartRippleEffect(rippleEffectDuration);
        //currentCubeHeight = playerMovement.GetCubeStandingOn().transform.localScale.y;
        Invoke(nameof(LevelUp), rippleEffectDuration/2);
    }

    void ComputeLevelingUp()
    {
        GameObject cubeUnderPlayer = playerMovement.GetCubeStandingOn();
        if (cubeUnderPlayer == null) return;
    }

    void LevelUp()
    {
        playerMovement.EnableMovement();
        if (currentLevel + 1 >= neededDiamondsPerLevel.Count) return;
        currentLevel++;
        artifactManager.ChangeCurrentLevel(currentLevel);
        levelingUp = false;
        SetNoiseMax();
    }

    void SetDiamondProgression()
    {
        artifactManager.SetNeededDiamonds(neededDiamondsPerLevel);
    }

    void SetNoiseMax()
    {
        noiseManager.SetMaxArousal(feelingRadiusPerLevel[currentLevel]);
        noiseManager.SetMaxValence(feelingRadiusPerLevel[currentLevel]);
    }
}
