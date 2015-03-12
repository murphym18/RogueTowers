using UnityEngine;
using System.Collections.Generic;
public class BulletScript : MonoBehaviour {


	public List<string> ThingsToDieOn = new List<string>();
	public float damage = 1.0f;
	public float speed = 3.0f;

	// Use this for initialization
	void Start () {
		rigidbody2D.velocity *= speed;
		Destroy (gameObject, 6);
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
		}
		
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (ThingsToDieOn.Contains(coll.gameObject.tag))
		{
			// stop and attack
			
			Destroy(gameObject, 0.0f);
		}
	}
}