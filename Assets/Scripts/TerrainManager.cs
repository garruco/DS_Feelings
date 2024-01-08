using UnityEngine;
using Noise;
using TMPro;
using System.Linq;

namespace Terrain
{
    public class TerrainManager : MonoBehaviour
    {
        [Header("Static")]
        public int nCubesSide;
        public float cubeEdgeMeasure;
        public Material material;

        public bool isFlat;

        [Header("UI")]
        public TextMeshProUGUI valenceUI;
        public TextMeshProUGUI arousalUI;

        private Vector3[] referenceHues;
        private Color cubeColor;
        private float lastHue;
        public Transform characterPos;

        public NoiseManager noiseManager;
        private float heightThreshold;
        private float timeTracker;

        [Header("LevelUp")]
        private bool rippling = false;
        private float ripplingDuration;
        private float ripplingValue;

        private GameObject[] cubes;
        // Start is called before the first frame update
        void Awake()
        {
            noiseManager = GameObject.FindGameObjectWithTag("NoiseManager").GetComponent<NoiseManager>();
            cubes = CreateCubes(nCubesSide, cubeEdgeMeasure, material);

            referenceHues = new[] { new Vector3 (-1,  1, 360),   new Vector3(0, -1, 30),  new Vector3 (1,  1,  60),
                                    new Vector3 (-1,  0, 300),                          new Vector3 (1,  0,  90),
                                    new Vector3 (-1, -1, 240), new Vector3(0,  1, 200), new Vector3 (1, -1, 160)
                                  };
        }

        // Update is called once per frame
        void Update()
        {
            float arousalValue = noiseManager.GetArousalRaw();
            float valenceValue = noiseManager.GetValenceRaw();

            float arousalValueEdited = noiseManager.GetArousalEdited();
            float valenceValueEdited = noiseManager.GetValenceEdited();

            TeleportTerrain();

            cubeColor = GetCubeColor(valenceValueEdited, arousalValueEdited);
            material.SetColor("_BaseColor", cubeColor);

            timeTracker += Time.deltaTime * arousalValue;
            float sinValue = Mathf.Sin(timeTracker) / 20;

            if (rippling)
            {
                ripplingDuration -= Time.deltaTime;

                ripplingValue += 0.15f;
                if (ripplingDuration <= 0)
                {
                    rippling = false;
                }
            }

            UpdateCubes(material, sinValue, valenceValueEdited, arousalValueEdited);

            UpdateUI(valenceValueEdited, arousalValueEdited);

        }

        Color GetCubeColor(float valence, float arousal) //TEST
        {
            float finalSaturation = Vector2.Distance(Vector2.zero, new Vector2(valence, arousal));
            float finalValue = ((valence + 1) / 2) * 0.75f + 0.25f;
            float finalHue;
            float hue = 0;

            Vector2 valenceArousalPos = new Vector2(valence, arousal);

            Vector2[] distancesToHues = new Vector2[referenceHues.Length];

            for (int i = 0; i < referenceHues.Length; i++)
            {
                distancesToHues[i] = new Vector2(Vector2.Distance(valenceArousalPos, new Vector2(referenceHues[i].x, referenceHues[i].y)), referenceHues[i].z); //get distance and store hue    
            }

            distancesToHues = distancesToHues.OrderBy(v => v.x).ToArray<Vector2>();

            int nPertinentHues = 4;
            float sumDistanceValues = 0;

            for (int i = 0; i < nPertinentHues; i++)
            {
                float val = 1 / distancesToHues[i].x;
                if (distancesToHues[i].x == 0) val = 1;
                distancesToHues[i] = new Vector2(val, distancesToHues[i].y);
                sumDistanceValues += val;
            }

            for (int i = 0; i < nPertinentHues; i++)
            {
                float val = distancesToHues[i].x / sumDistanceValues;
                hue += val * distancesToHues[i].y;
            }

            finalHue = lastHue + (hue - lastHue) * 0.01f;

            lastHue = finalHue;

            return Color.HSVToRGB(finalHue / 360, finalSaturation, finalValue);
        }

        void TeleportTerrain()
        {
            float distanceXToPlayer = transform.position.x - characterPos.position.x;

            float xToMove = 0;
            if (Mathf.Abs(distanceXToPlayer) > cubeEdgeMeasure)
            {
                int edgesToMove = Mathf.FloorToInt(Mathf.Abs(distanceXToPlayer) / cubeEdgeMeasure);
                if (distanceXToPlayer < 0) edgesToMove *= -1;
                xToMove = edgesToMove * cubeEdgeMeasure;
            }

            float distanceZToPlayer = transform.position.z - characterPos.position.z;

            float zToMove = 0;
            if (Mathf.Abs(distanceZToPlayer) > cubeEdgeMeasure)
            {
                int edgesToMove = Mathf.FloorToInt(Mathf.Abs(distanceZToPlayer) / cubeEdgeMeasure);
                if (distanceZToPlayer < 0) edgesToMove *= -1;
                zToMove = edgesToMove * cubeEdgeMeasure;
            }
            transform.position = new Vector3(transform.position.x - xToMove, transform.position.y, transform.position.z - zToMove);
        }

        void UpdateCubes(Material mat, float sinValue, float valence, float arousal)
        {
            float scale = (Mathf.Pow(arousal + 1, 3)) * 1.5f;
            float erraticness = Vector2.Distance(new Vector2(-1, -1), new Vector2(valence, arousal)) * 0.15f;
            float rippleSpeed = 0.5f;

            for (int x = 0; x < nCubesSide; x++)
            {
                for (int z = 0; z < nCubesSide; z++)
                {
                    int currentIndex = x * nCubesSide + z;

                    float distance = Vector3.Distance(cubes[currentIndex].transform.position, characterPos.position) + erraticness;
                    float distanceValue = distance / 10;
                    //float distanceValue = 1 / 1 + Mathf.Pow(100000, 1 - distance);
                    //float distanceValue = 2;

                    float xDistance = cubes[currentIndex].transform.position.x - characterPos.position.x;
                    float zDistance = cubes[currentIndex].transform.position.z - characterPos.position.z;

                    float currentCubeHeight = 0;

                    //float currentCubeHeight = distanceValue * scale;
                    if (isFlat) currentCubeHeight = Mathf.PerlinNoise(xDistance * erraticness + sinValue, zDistance * erraticness + sinValue) * distanceValue * scale + 0.1f;
                    else currentCubeHeight = Mathf.PerlinNoise(xDistance * erraticness + sinValue, zDistance * erraticness + sinValue) * scale;

                    if (rippling)
                    {
                        float rippleDistanceValue = Mathf.Abs(distance - ripplingValue);
                        if (rippleDistanceValue < 4)
                        {
                            float rippleValue = Mathf.Clamp(1 / Mathf.Abs(distance - ripplingValue), 0, ripplingValue/4);
                            currentCubeHeight += rippleValue;
                        }
                    }

                    //float currentCubeHeight = Mathf.PerlinNoise(xDistance * erraticness + sinValue, zDistance * erraticness + sinValue) * 0.75f * scale;
                    //if (currentCubeHeight < 0.1f) currentCubeHeight = 0.1f;
                    cubes[currentIndex].transform.position = new Vector3(cubes[currentIndex].transform.position.x, currentCubeHeight / 2, cubes[currentIndex].transform.position.z);
                    cubes[currentIndex].transform.localScale = new Vector3(cubeEdgeMeasure, currentCubeHeight, cubeEdgeMeasure);
                }
            }
        }

        void UpdateUI(float valence, float arousal)
        {
            valenceUI.text = "Valence: " + valence;
            arousalUI.text = "Arousal: " + arousal;
        }

        void UpdateShader(Color color)
        {
            material.SetColor("Color", color);
        }

        GameObject[] CreateCubes(int nCubesSide, float cubeEdgeMeasure, Material mat)
        {
            int nTotalCubes = nCubesSide * nCubesSide;

            GameObject[] createdCubes = new GameObject[nTotalCubes];

            for (int x = 0; x < nCubesSide; x++)
            {
                float currentXPos = (x - nCubesSide / 2) * cubeEdgeMeasure;

                for (int z = 0; z < nCubesSide; z++)
                {
                    float currentZPos = (z - nCubesSide / 2) * cubeEdgeMeasure;
                    int currentIndex = x * nCubesSide + z;

                    GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    newCube.transform.position = new Vector3(currentXPos, 0, currentZPos);
                    newCube.transform.localScale = new Vector3(cubeEdgeMeasure, cubeEdgeMeasure, cubeEdgeMeasure);
                    newCube.GetComponent<Renderer>().material = mat;
                    newCube.transform.parent = gameObject.transform;
                    newCube.name = "cube_" + currentIndex;
                    newCube.layer = LayerMask.NameToLayer("whatIsGround");

                    createdCubes[currentIndex] = newCube;
                }
            }
            return createdCubes;
        }

        public void IncreaseCubeHeight(GameObject cube, float height)
        {
            cube.transform.localScale = new Vector3(cubeEdgeMeasure, height, cubeEdgeMeasure);
        }

        public void StartRippleEffect(float duration) //ripple effect on level up
        {
            ripplingValue = 0;
            ripplingDuration = duration;
            rippling = true;
        }

        public float GetHue()
        {
            return lastHue;
        }
    }
}
