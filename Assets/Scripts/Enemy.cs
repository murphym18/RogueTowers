using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	public float movementSpeed;
	
	private Transform target;
	private Rigidbody2D rb2D;
	
	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag ("Cage").transform;
		rb2D = GetComponent <Rigidbody2D> (); // can get rid of?
		MoveSimple();
	}
	
	// Update is called once per frame
	void Update () {
		MoveSimple();
	}
	
	void MoveSimple() {
//		Vector3 newPostion = Vector3.MoveTowards(rb2D.position, target.position, movementSpeed);
		//rb2D.MovePosition (newPostion);
		
		Vector3 direction = target.rigidbody2D.position - this.rigidbody2D.position;
		Vector3 velocity = direction.normalized * movementSpeed;
		rigidbody2D.velocity = velocity;
	}
}
