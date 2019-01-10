using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public Game game;
    public Rigidbody rb;

    public Vector3 startPos;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void useGravity(bool use)
    {
        rb.useGravity = use;
    }

    public void resetPosition()
    {
        transform.position = startPos;
        rb.velocity = new Vector3(0, 0, 0);
    }
}
