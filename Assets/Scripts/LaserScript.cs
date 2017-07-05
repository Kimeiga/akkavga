using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {

	/*
	 * Four behaviors to laser:
	 * 1 emerging from the cannon (For now, I don't think I need to implement this)
	 * 2 flying through space
	 * 3 reflecting off walls
	 * 4 hitting enemy
	 */


	public float speed;



	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {

		//behavior 2

		transform.Translate (0, speed, 0);


		//Raycasting
		//if(Physics2D.Raycast(transform.position,
	}
}
