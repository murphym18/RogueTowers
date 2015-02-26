using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	public Collider2D TestSpawn;
	public float movementSpeed = 5.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
		if (!Input.anyKey)
		{
			rigidbody2D.velocity = Vector2.zero;
		}

		if (Input.GetKey(KeyCode.A)) {
			rigidbody2D.velocity = (rigidbody2D.velocity + new Vector2(-1,0)).normalized * movementSpeed;
		}
		if (Input.GetKey(KeyCode.W)) {
			rigidbody2D.velocity = (rigidbody2D.velocity + new Vector2(0,1)).normalized * movementSpeed;
		}
		if (Input.GetKey(KeyCode.D)) {
			rigidbody2D.velocity = (rigidbody2D.velocity + new Vector2(1,0)).normalized * movementSpeed;
		}
		if (Input.GetKey(KeyCode.S)) {
			rigidbody2D.velocity = (rigidbody2D.velocity + new Vector2(0,-1)).normalized * movementSpeed;
		}
		/*
		if (Input.GetKeyDown(KeyCode.A)) {
			transform.position = new Vector3 (rigidbody2D.position.x - 1, rigidbody2D.position.y, 0);
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			transform.position = new Vector3 (rigidbody2D.position.x, rigidbody2D.position.y + 1, 0);
		}
		if (Input.GetKeyDown(KeyCode.D)) {
			transform.position = new Vector3 (rigidbody2D.position.x + 1, rigidbody2D.position.y, 0);
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			transform.position = new Vector3 (rigidbody2D.position.x, rigidbody2D.position.y -1, 0);
		}
		*/
	}
	void FixedUpdate(){
		if (Input.GetKeyDown (KeyCode.J)) {
			Collider2D attackInstance = Instantiate (TestSpawn, transform.position + new Vector3 (1, 0, 0), Quaternion.Euler (new Vector3 (0, 0, 0))) as Collider2D;  
		}
	}
}
