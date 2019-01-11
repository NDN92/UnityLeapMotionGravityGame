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
        Debug.Log(Microphone.devices[0]);

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
        int sampleWindow = 25;        
        int micPos = Microphone.GetPosition(micName) - (sampleWindow + 1);
        if(micPos < 0)
        {
            return;
        }

        float[] waveData = new float[sampleWindow];
        audio.clip.GetData(waveData, micPos);
        float levelMax = 0;
        float levelMax_abs = 0;
        string temp = "";
        for (int i = 0; i < waveData.Length; i++)
        {
            temp += waveData[i] + ", ";
            levelMax_abs += Mathf.Abs(waveData[i]);

            if (levelMax < waveData[i])
            {
                levelMax = waveData[i];
            }
        }
        levelMax_abs = levelMax_abs / sampleWindow;

        
        //Debug.Log(temp);
        //Debug.Log(levelMax);
        //Debug.Log(levelMax_abs);
        

        loudness = levelMax_abs;
    }

    public float getLoudness()
    {
        return loudness;
    }
}
