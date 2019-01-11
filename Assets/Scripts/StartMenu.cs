using UnityEngine;
using System.Collections;
using Leap;

public class StartMenu : MonoBehaviour
{
    public Game game;
    public GameObject bg;
    public GameObject text_gameTitle;
    public GameObject leftHandModel;
    public GameObject leftHandModel_Mesh;
    public GameObject rightHandModel;
    public GameObject rightHandModel_Mesh;

    private float twoHandsTime;

    // Use this for initialization
    void Start()
    {
        twoHandsTime = 0f;
    }

    private void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnApplicationQuit()
    {
        noHandDetected();
    }

    public void noHandDetected()
    {
        leftHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
        rightHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);

        twoHandsTime = 0f;
        game.setIsStartingProcedure(false);
    }

    public void oneHandDetected(Hand hand)
    {
        if(hand.IsLeft)
        {
            leftHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
            rightHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
        } else
        {
            rightHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
            leftHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
        }

        twoHandsTime = 0f;
        game.setIsStartingProcedure(false);
    }

    public void twoHandsDetected(float deltaTime)
    {
        if(twoHandsTime < 0.000001f)
        {
            leftHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
            rightHandModel_Mesh.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
        }
        
        if(bg.transform.position.z > -17f)
        {
            bg.transform.position = new Vector3(bg.transform.position.x, bg.transform.position.y, bg.transform.position.z - 0.2f);
            text_gameTitle.transform.position = new Vector3(text_gameTitle.transform.position.x, text_gameTitle.transform.position.y, text_gameTitle.transform.position.z - 0.2f);
            leftHandModel.transform.position = new Vector3(leftHandModel.transform.position.x, leftHandModel.transform.position.y, leftHandModel.transform.position.z - 0.2f);
            rightHandModel.transform.position = new Vector3(rightHandModel.transform.position.x, rightHandModel.transform.position.y, rightHandModel.transform.position.z - 0.2f);

            if (!game.isStartingProcedure() && bg.transform.position.z < -8f)
            {
                game.setIsStartingProcedure(true);
            }
        } else
        {
            twoHandsTime += deltaTime;

            game.startGame();
        }
    }
}
