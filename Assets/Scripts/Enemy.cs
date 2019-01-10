using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponents<Ball>().Length > 0)
        {
            Ball ball = collision.gameObject.GetComponents<Ball>()[0];
            ball.resetPosition();
            Destroy(gameObject);
        }
        if (collision.gameObject.GetComponents<Ground>().Length > 0)
        {
            Destroy(gameObject);
        }
    }
}
