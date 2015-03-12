using UnityEngine;
using System.Collections.Generic;
public class BishopBulletScript : BulletScript {
	Collider2D[] enemies;
	float enemyToAttackDistance = 3.0f;
	float lastAttack = 0.0f;
	int enemyToAttackIndex = 0;
	float attackRadius = 10.0f;
	public LayerMask whatIsTargetable;


	BishopBulletScript() {
		damage = 7.0f;
		speed = 9.0f;
	}

	void Start() {
		rigidbody2D.velocity *= speed;
		//Destroy (gameObject, 6);
		GameObject[] Towers = GameObject.FindGameObjectsWithTag("Tower");
		for(int i = 0; Towers != null && i < Towers.Length; i++) {
			Physics2D.IgnoreCollision(collider2D, Towers[i].collider2D);
		}
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
		if (ThingsToDieOn.Contains(coll.gameObject.tag) && coll.gameObject.tag == "Enemy")
		{
			//Explosion!!!
			Destroy(gameObject, 0.0f);
		} else if (ThingsToDieOn.Contains(coll.gameObject.tag) && coll.gameObject.tag == "Cage" || coll.gameObject.tag == "Wall"){
			//Bounce!!!
			Bounce (coll.transform);
		}
	}

	void Bounce(Transform coll){
		if(Mathf.FloorToInt(coll.position.x) > Mathf.FloorToInt(transform.position.x))
			rigidbody2D.velocity = new Vector3(-rigidbody2D.velocity.x, rigidbody2D.velocity.y, 0);
		else if(Mathf.FloorToInt(coll.position.y) > Mathf.FloorToInt(transform.position.y))
			rigidbody2D.velocity = new Vector3(rigidbody2D.velocity.x, -rigidbody2D.velocity.y, 0);
		if(Mathf.FloorToInt(coll.position.x) < Mathf.FloorToInt(transform.position.x))
			rigidbody2D.velocity = new Vector3(-rigidbody2D.velocity.x, rigidbody2D.velocity.y, 0);
		else if(Mathf.FloorToInt(coll.position.y) < Mathf.FloorToInt(transform.position.y))
			rigidbody2D.velocity = new Vector3(rigidbody2D.velocity.x, -rigidbody2D.velocity.y, 0);
	}
}