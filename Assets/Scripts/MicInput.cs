using UnityEngine;
using System.Collections;

public class MicInput : MonoBehaviour
{
    private AudioSource audio;
    private string micName;

    private float loudness;

    void Start()
    {
        //string[] mics = Microphone.devices;

        audio = GetComponent<AudioSource>();        
        micName = Microphone.devices[0];
        audio.clip = Microphone.Start(micName, true, 10, 44100);
        audio.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { };
        audio.Play();

        loudness = 0;
    }

    void Update()
    {
        int sampleWindow = 128;        
        int micPos = Microphone.GetPosition(micName) - (sampleWindow + 1);
        if(micPos < 0)
        {
            return;
        }

        float[] waveData = new float[sampleWindow];
        audio.clip.GetData(waveData, micPos);
        float levelMax = 0;
        for (int i = 0; i < sampleWindow; i++)
        {
            if (levelMax < waveData[i])
            {
                levelMax = waveData[i];
            }
        }

        loudness = levelMax;
    }

    public float getLoudness()
    {
        return loudness;
    }
}
