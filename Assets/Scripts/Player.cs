using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class Player : MonoBehaviour {

    public Game game;

    public float speed = 10f;
    public Rigidbody rb;

    public Vector3 startPos;
    private Vector3 startScale;

    // Use this for initialization
    void Start () {
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate() {

    }

    public void _update()
    {
        Vector3 leftHandPos = game.getLeftHandPos();
        Vector3 rightHandPos = game.getRightHandPos() ;
        Vector3 leap_leftHandPos = game.getLeap_leftHandPos();
        Vector3 leap_rightHandPos = game.getLeap_rightHandPos();

        Vector3 targetPos;
        float centerX = 0;
        float centerY = 0;
        float targetAngleInDeg = 0;
        float currentAngleInDegree = 0;
        float width = 2f;

        // Position
        if (leftHandPos.y < rightHandPos.y)
        {
            centerY = leftHandPos.y + ((rightHandPos.y - leftHandPos.y) / 2f);
        }
        else
        {
            centerY = rightHandPos.y + ((leftHandPos.y - rightHandPos.y) / 2f);
        }
        centerX = leftHandPos.x + ((rightHandPos.x - leftHandPos.x) / 2f);
        targetPos = new Vector3(centerX, centerY, transform.position.z);
        //Außerhalb des Spielbereiches X
        if (targetPos.x < game.rangeLevelMin.x)
        {
            targetPos.x = game.rangeLevelMin.x;
        } else if(targetPos.x > game.rangeLevelMax.x)
        {
            targetPos.x = game.rangeLevelMax.x;
        }
        //Außerhalb des Spielbereiches Y
        if (targetPos.y < game.rangeLevelMin.y)
        {
            targetPos.y = game.rangeLevelMin.y;
        }
        else if (targetPos.y > game.rangeLevelMax.y)
        {
            targetPos.y = game.rangeLevelMax.y;
        }
        //Debug.Log(targetPos.z);
        Vector3 posForce = targetPos - transform.position;
        //rb.AddForce(force.x, force.y, force.z, ForceMode.Force);
        rb.velocity = posForce * 20;
        //transform.position = new Vector3(centerX, centerY, transform.position.z);


        // Tilt
        targetAngleInDeg = Mathf.Atan((leap_rightHandPos.y - leap_leftHandPos.y) / (leap_rightHandPos.x - leap_leftHandPos.x)) * (180f/Mathf.PI);
        //Debug.Log(targetAngleInDeg);
        currentAngleInDegree = transform.rotation.eulerAngles.z;
        if (currentAngleInDegree > 180)
        {
            currentAngleInDegree = currentAngleInDegree - 360;
        }
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, tiltInDeg);            
        Vector3 angularForce = new Vector3(0, 0, (targetAngleInDeg - currentAngleInDegree) / 5f );
        rb.angularVelocity = angularForce;


        //Size
        float width_x = (leap_rightHandPos.x - leap_leftHandPos.x) / 1.8f;
        //width_x = Mathf.Log()
        width = game.convertLeapSpaceToWorldSpace(new Vector3(width_x, 0, 0)).x - 10f;
        transform.localScale = new Vector3(width, transform.localScale.y, transform.localScale.z);
        //Debug.Log(width);
    }

    public void resetPosition()
    {
        transform.position = startPos;
        transform.localScale = startScale;
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }
}
