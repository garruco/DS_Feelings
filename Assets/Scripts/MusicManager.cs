using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Noise;

public class MusicManager : MonoBehaviour
{
    public NoiseManager noiseManager;
    private Vector2[] musicNoiseLocations;
    public AudioSource[] sources;

    void Awake()
    {
        musicNoiseLocations = new[] { new Vector2 (-1,  1), new Vector2 (1,  1),
                                      new Vector2 (-1, -1), new Vector2 (1, -1)
                                  };

    }
    void Start()
    {
        for(int i = 0; i < sources.Length; i++)
        {
            sources[i].Play(0);
        }
    }

    void Update()
    {
        float valenceValue = noiseManager.GetValenceEdited();
        float arousalValue = noiseManager.GetArousalEdited();
        CalculateMusicVolumes(valenceValue, arousalValue);
    }

    void CalculateMusicVolumes(float valence, float arousal)
    {
        Vector2 valenceArousalPos = new Vector2(valence, arousal);

        Vector2[] distancesToMusic = new Vector2[musicNoiseLocations.Length];

        for (int i = 0; i < distancesToMusic.Length; i++)
        {
            distancesToMusic[i] = new Vector2(Vector2.Distance(valenceArousalPos, musicNoiseLocations[i]), i); //get distance and store index of music    
        }

        float a = 0.6f;
        float b = -0.5f;

        for (int i = 0; i < distancesToMusic.Length; i++)
        {
            float val = ((1 * a) / distancesToMusic[i].x + b) * 0.6f;
            sources[i].volume = val;//(musicNames[(int)distancesToMusic[i].y], val);
        }
    }
}
