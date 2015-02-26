using UnityEngine;
using System.Collections;
public class BulletScript : MonoBehaviour {
	// Use this for initialization
	void Start () {
		Destroy (gameObject, 3);
	}
	
	// Update is called once per frame
	void Update () {
		//Add destroy on enemy collision
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Enemy")
		{
			// stop and attack
			//rigidbody2D.velocity = Vector3.zero;
			Destroy(gameObject, 0.0f);
		}
		
	}
}