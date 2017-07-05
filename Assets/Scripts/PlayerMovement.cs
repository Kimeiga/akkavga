using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	//Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

	public float speed;

	public float stopDistance;

	public Rigidbody2D rigid;

	public GameObject laser;


	void Start() {
		rigid = GetComponent<Rigidbody2D> ();
	}

	void Update(){

		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		if (Input.GetMouseButtonDown (0)) {
			Instantiate (laser, transform.position, Quaternion.LookRotation (transform.position - mousePosition, 
				Vector3.forward));
		}

	}

	void FixedUpdate(){
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);


		Quaternion rot = Quaternion.LookRotation (transform.position - mousePosition, 
			Vector3.forward);

		transform.rotation = rot;

		transform.eulerAngles = new Vector3 (0, 0, transform.eulerAngles.z);
		rigid.angularVelocity = 0f;

		Vector3 flatMousePosition = mousePosition;
		flatMousePosition.z = 0;

		if(Vector3.Distance(transform.position, flatMousePosition) > stopDistance){

			float input = Input.GetAxis ("Vertical");
			rigid.AddForce (gameObject.transform.up * speed * input);
		}




	}
}
