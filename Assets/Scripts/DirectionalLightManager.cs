using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Noise;

public class DirectionalLightManager : MonoBehaviour
{
    Light myLight;
    private NoiseManager noiseManager;

    public float minimumIntensity;
    public float maximumIntensity;

    public float minimumTemperature;
    public float maximumTemperature;

    private float intensity;
    private float temperature;
    
    void Awake()
    {
        myLight = GetComponent<Light>();
        noiseManager = GameObject.FindGameObjectWithTag("NoiseManager").GetComponent<NoiseManager>();
        gameObject.GetComponent<Light>().intensity = minimumIntensity;
    }

    private void LateUpdate()
    {
        float valence = (noiseManager.GetValenceRaw() + 1) / 2;
        float currentIntensity = minimumIntensity + valence * (maximumIntensity-minimumIntensity);
        intensity += (currentIntensity - intensity) * 0.05f;
        gameObject.GetComponent<Light>().intensity = intensity;



        float arousal = 1 - (noiseManager.GetArousalRaw() + 1) / 2; //higher arousal means lower temperature (warmer)
        float currentTemperature = minimumTemperature + valence * (maximumTemperature-minimumTemperature);
        temperature += (temperature - currentTemperature) * 0.2f;
        gameObject.GetComponent<Light>().colorTemperature = temperature;

        Debug.Log(valence);
    }
}
