using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Noise
{
    public class NoiseManager : MonoBehaviour
    {
        public Transform playerPos;
        [Range(0, 1)]
        public float maxArousalValue;
        private float targetMaxArousal;
        private float arousalValue = 0;
        [Range(0, 1)]
        public float maxValenceValue;
        private float targetMaxValence;
        private float valenceValue = 0;
        float arousalValueEdited;
        float valenceValueEdited;
        public float mainValuesNoiseInc;
        public float startingTime;
        private float timeTracker;

        private float seed;
        private float secondSeed;
        void Awake()
        {
            seed = Random.Range(0, 10000);
            secondSeed = Mathf.PerlinNoise(seed, seed) + 10000 * 20000;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            timeTracker += Time.deltaTime;

            float noiseOffsetX = playerPos.position.x * mainValuesNoiseInc;
            float noiseOffsetZ = playerPos.position.z * mainValuesNoiseInc;

            maxArousalValue += (targetMaxArousal - maxArousalValue) * 0.002f;
            maxValenceValue += (targetMaxValence - maxValenceValue) * 0.002f;

            arousalValue = Map(-1 + Mathf.PerlinNoise(seed + noiseOffsetX, noiseOffsetZ)                     * 2, -1, 1, -maxArousalValue, maxArousalValue);
            valenceValue = Map(-1 + Mathf.PerlinNoise(seed + noiseOffsetX + secondSeed, noiseOffsetZ + 1000) * 2, -1, 1, -maxValenceValue, maxValenceValue);

            //arousalValue = 1;
            //valenceValue = -1;

/*             if (timeTracker < startingTime)
            {
                float multiplier = timeTracker / startingTime;
                arousalValue *= multiplier;
                valenceValue *= multiplier;
            } */

            arousalValueEdited = Map(1 / (1 + Mathf.Pow(234, -arousalValue)) * 2 - 1, -1, 1, -maxArousalValue, maxArousalValue); //through sigmoid graph for more extreme results
            valenceValueEdited = Map(1 / (1 + Mathf.Pow(234, -valenceValue)) * 2 - 1, -1, 1, -maxArousalValue, maxArousalValue);
        }

        public void SetMaxArousal(float newMaxArousal){
            targetMaxArousal = newMaxArousal;
        }
        public void SetMaxValence(float newMaxValence){
            targetMaxValence = newMaxValence;
        }

        public float Map(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public float GetArousalRaw()
        {
            return arousalValue;
        }
        public float GetArousalEdited()
        {
            return arousalValueEdited;
        }
        public float GetValenceRaw()
        {
            return valenceValue;
        }
        public float GetValenceEdited()
        {
            return valenceValueEdited;
        }

        public void ResetValues() //TO-DO
        {
            valenceValue = 0;
            arousalValue = 0;
        }
    }
}
