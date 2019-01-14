using UnityEngine;
using System.Collections;

public class TextObj : MonoBehaviour
{
    public TextMesh textMesh;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateText(string text)
    {
        textMesh.text = text;
    }

    public void updateText_Time(float time)
    {
        int mins = (int)Mathf.Round(time / 60f);
        int secs = (int)Mathf.Round(time % 60f);

        string text = "";

        if(mins < 10)
        {
            text += "0";
        }
        text += mins + ":";
        if(secs < 10)
        {
            text += "0";
        }
        text += secs;

        textMesh.text = text;
    }

    public string getText()
    {
        return textMesh.text;
    }
}
