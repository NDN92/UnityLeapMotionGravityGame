using UnityEngine;
using System.Collections;

public class ScreenDarken : MonoBehaviour
{
    public Material material;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setOpacity(float opacity)
    {
        material.color = new Color(0, 0, 0, opacity);
    }
}
