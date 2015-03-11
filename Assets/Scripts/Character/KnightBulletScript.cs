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

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (ThingsToDieOn.Contains(coll.gameObject.tag) && coll.gameObject.tag == "Enemy")
		{
			// stop and attack
			
			if(hitsLeft > 0) {
				enemies = Physics2D.OverlapCircleAll(rigidbody2D.position, attackRadius, whatIsTargetable, 0, 0);
				enemyToAttackIndex = -1;
				enemyToAttackDistance = attackRadius;
				for (int i = 0; enemies != null && i < enemies.Length; i++) {
					if((enemies[i].transform.position - this.transform.position).magnitude < enemyToAttackDistance && enemies[i].gameObject != lastHitEnemy){
						enemyToAttackIndex = i;
						enemyToAttackDistance = (enemies[i].transform.position - this.transform.position).magnitude;
					}
				}

				if(enemyToAttackIndex != -1) {
					lastHitEnemy = enemies[enemyToAttackIndex].gameObject;
					rigidbody2D.velocity = (enemies[enemyToAttackIndex].transform.position - this.transform.position).normalized*speed;
					hitsLeft--;
				}
			} else {
				Destroy(gameObject, 0.0f);
				Debug.Log("Bullet gone\n");
			}
		} else if (ThingsToDieOn.Contains(coll.gameObject.tag) && coll.gameObject.tag != "Enemy") {
			Destroy(gameObject, 0.0f);
		}
	}
}