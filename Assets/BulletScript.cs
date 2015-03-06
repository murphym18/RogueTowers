using UnityEngine;
using System.Collections.Generic;
public class BulletScript : MonoBehaviour {
	public List<string> ThingsToDieOn = new List<string>();
	public float damage = 1.0f;

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

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (ThingsToDieOn.Contains(coll.gameObject.tag))
		{
			// stop and attack
			
			Destroy(gameObject, 0.0f);
		}
	}
}