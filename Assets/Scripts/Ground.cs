using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

    public Game game;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponents<Ball>().Length > 0)
        {
            Ball ball = collision.gameObject.GetComponents<Ball>()[0];
            ball.resetPosition();

            game.lifesMinusOne();
        }
    }
}
