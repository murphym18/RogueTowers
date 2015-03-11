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
		Debug.Log ("BOUNCE!");
		if(coll.position.x > transform.position.x)
			rigidbody2D.velocity = new Vector3(-rigidbody2D.velocity.x, rigidbody2D.velocity.y, 0);
		else if(coll.position.y > transform.position.y)
			rigidbody2D.velocity = new Vector3(rigidbody2D.velocity.x, -rigidbody2D.velocity.y, 0);
		else if(coll.position.x < transform.position.x)
			rigidbody2D.velocity = new Vector3(-rigidbody2D.velocity.x, rigidbody2D.velocity.y, 0);
		else if(coll.position.y < transform.position.y)
			rigidbody2D.velocity = new Vector3(rigidbody2D.velocity.x, -rigidbody2D.velocity.y, 0);
	}
}