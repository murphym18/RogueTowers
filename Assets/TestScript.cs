using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	public Collider2D TestSpawn;

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
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
	}
	void FixedUpdate(){
		if (Input.GetKeyDown (KeyCode.J)) {
			Collider2D attackInstance = Instantiate (TestSpawn, transform.position + new Vector3 (1, 0, 0), Quaternion.Euler (new Vector3 (0, 0, 0))) as Collider2D;        
		}
	}
}
