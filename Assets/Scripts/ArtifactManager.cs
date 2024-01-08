using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Noise;
using Player;
using Terrain;

namespace Artifact
{
    public class ArtifactManager : MonoBehaviour
    {
        public GameObject diamond;
        public Material diamondMaterial;
        public Material terrainMaterial;
        public NoiseManager noiseManager;
        public TerrainManager terrainManager;
        public PlayerMovement playerMovement;
        public float minDistanceForGeneration;
        public float maxDistance;
        public RelicParent relicParent;
        public List<Diamond> diamonds;
        private bool diamondPhase = true;

        [Header("Level Progression")]
        private List<int> neededDiamondsPerLevel;
        private int currentLevel = 0;
        private int currentLevelCollectedDiamonds;
        public delegate void RelicAction();
        public static event RelicAction OnRelicCollection;
        void Awake()
        {
            StatusRelic(false);
            for (int i = 0; i < diamonds.Count; i++)
            {
                GenerateDiamond(i);
            }
        }
        void Update()
        {
            if (diamondPhase && !relicParent.GetCollectionStatus())
            {
                int? indexDiamondToReset = CheckDiamondNeedsReset();
                if (indexDiamondToReset != null)
                {
                    GenerateDiamond((int)indexDiamondToReset);
                }

                ChangeDiamondsMaterial();
            }
            else
            {
                if (relicParent.GetCollectionStatus())
                {
                    Invoke(nameof(RelicCollected), 2);
                    relicParent.SetCollectionFalse();
                }
                else if (CheckRelicNeedsReset()) GenerateRelic();
            }
        }

        private void RelicCollected()
        {
            diamondPhase = true;
            GenerateAllDiamonds();
            StatusDiamonds(true);
            StatusRelic(false);
            if (OnRelicCollection != null)
                OnRelicCollection();
        }

        void ChangeDiamondsMaterial()
        {
            Color terrainColor = terrainMaterial.GetColor("_BaseColor");
            float terrainColorH, terrainColorS, terrainColorV;
            Color.RGBToHSV(terrainColor, out terrainColorH, out terrainColorS, out terrainColorV);
            Color diamondColor = Color.HSVToRGB(terrainColorH, 1, 1);
            diamondMaterial.SetColor("_BaseColor", diamondColor);
        }

        private int? CheckDiamondNeedsReset()
        {
            for (int i = 0; i < diamonds.Count; i++)
            {
                if (diamonds[i].GetCollectionStatus())
                {
                    currentLevelCollectedDiamonds++;
                    CheckAllDiamondsCollected();
                    return i;
                }
                if (Vector2.Distance(playerMovement.GetPosFlat(), diamonds[i].GetPosFlat()) > maxDistance)
                {
                    return i;
                }
                int? diamondTooClose = CloseToOtherDiamonds(diamonds[i].GetPosFlat(), i);
                if (diamondTooClose != null) return (int)diamondTooClose;
            }
            return null;
        }

        private bool CheckRelicNeedsReset()
        {
            if (Vector2.Distance(playerMovement.GetPosFlat(), relicParent.GetPosFlat()) > maxDistance) return true;
            return false;
        }

        private void CheckAllDiamondsCollected()
        {
            if (currentLevelCollectedDiamonds >= neededDiamondsPerLevel[currentLevel])
            {
                StatusDiamonds(false);
                StatusRelic(true);
                GenerateRelic();
            }
        }

        private void GenerateAllDiamonds()
        {
            for (int i = 0; i < diamonds.Count; i++)
            {
                GenerateDiamond(i);
            }
        }
        private void GenerateDiamond(int index)
        {
            float distanceFlat = Random.Range(minDistanceForGeneration, maxDistance);
            float angle = Random.Range(0, Mathf.PI * 2);
            Vector2 playerPos = playerMovement.GetPosFlat();
            Vector3 newPos = new Vector3(playerPos.x + Mathf.Cos(angle) * distanceFlat, 200, playerPos.y + Mathf.Sin(angle) * distanceFlat);

            int? diamondTooClose = CloseToOtherDiamonds(newPos, index);

            if (diamondTooClose != null)
            {
                GenerateDiamond((int)diamondTooClose);
                return;
            }

            diamonds[index].Reset(newPos);
        }
        private void GenerateRelic()
        {
            float distanceFlat = Random.Range(minDistanceForGeneration, maxDistance);
            float angle = Random.Range(0, Mathf.PI * 2);
            Vector2 playerPos = playerMovement.GetPosFlat();
            Vector3 newPos = new Vector3(playerPos.x + Mathf.Cos(angle) * distanceFlat, 200, playerPos.y + Mathf.Sin(angle) * distanceFlat);

            relicParent.Reset(newPos);
        }

        private void StatusDiamonds(bool status)
        {
            diamondPhase = status;
            for (int i = 0; i < diamonds.Count; i++)
            {
                diamonds[i].gameObject.SetActive(status);
            }
        }

        private void StatusRelic(bool status)
        {
            relicParent.gameObject.SetActive(status);
        }

        private int? CloseToOtherDiamonds(Vector2 pos, int index)
        {
            for (int i = 0; i < diamonds.Count; i++)
            {
                if (i == index) continue;
                else if (Vector2.Distance(diamonds[i].GetPosFlat(), pos) < minDistanceForGeneration)
                {
                    if(Vector2.Distance(diamonds[i].GetPosFlat(), playerMovement.GetPosFlat()) < Vector2.Distance(pos, playerMovement.GetPosFlat()))
                    {
                        return index;
                    }
                    return i;
                }
            }

            return null;
        }

        public void SetNeededDiamonds(List<int> diamondProgression)
        {
            neededDiamondsPerLevel = diamondProgression;
        }

        public void ChangeCurrentLevel(int level)
        {
            Debug.Log("LEVEL UP: " + level);
            currentLevelCollectedDiamonds = 0;
            currentLevel = level;
        }
    }
}