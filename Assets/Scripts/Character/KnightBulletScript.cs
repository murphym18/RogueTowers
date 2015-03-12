using UnityEngine;
using System.Collections.Generic;
public class KnightBulletScript : BulletScript {
	Collider2D[] enemies;
	float enemyToAttackDistance = 3.0f;
	float lastAttack = 0.0f;
	int enemyToAttackIndex = 0;
	float attackRadius = 3.0f;
	public LayerMask whatIsTargetable;
	int hitsLeft = 3;
	GameObject lastHitEnemy = null;
	float attackLifeSpan = 0.5f;
	float attackRefreshedLife = 0.0f;
	bool hitFirstTarget = false;

	KnightBulletScript() {
		damage = 5.0f;
		speed = 10.0f;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Enemy")
		{
			// stop and attack
			//rigidbody2D.velocity = Vector3.zero;

		}
		
	}

	void FixedUpdate() {
		if(hitFirstTarget && Time.time < attackRefreshedLife + attackLifeSpan) {
			Destroy(gameObject, 0.5f);
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{


		if (ThingsToDieOn.Contains(coll.gameObject.tag) && coll.gameObject.tag == "Enemy")
		{

			if (!hitFirstTarget) {
				hitFirstTarget = true;
			}
			attackRefreshedLife = Time.time;
			// stop and attack
			
			if(hitsLeft > 0) {
				enemies = Physics2D.OverlapCircleAll(rigidbody2D.position, attackRadius, whatIsTargetable, 0, 0);
				enemyToAttackIndex = -1;
				enemyToAttackDistance = attackRadius;
				for (int i = 0; enemies != null && i < enemies.Length; i++) {
					if((enemies[i].transform.position - this.transform.position).magnitude < enemyToAttackDistance && enemies[i].gameObject != coll.gameObject){
						enemyToAttackIndex = i;
						enemyToAttackDistance = (enemies[i].transform.position - this.transform.position).magnitude;
					}
				}

				if(enemyToAttackIndex != -1) {
					rigidbody2D.velocity = (enemies[enemyToAttackIndex].transform.position - this.transform.position).normalized*speed;
					hitsLeft--;
				}
			} else {
				Destroy(gameObject, 0.0f);
				// Debug.Log("Bullet gone\n");
			}
		} else if (ThingsToDieOn.Contains(coll.gameObject.tag) && coll.gameObject.tag != "Enemy") {
			Destroy(gameObject, 0.0f);
		}
	}
}