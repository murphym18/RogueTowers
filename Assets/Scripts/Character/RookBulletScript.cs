using System;
using UnityEngine;
using System.Collections.Generic;
public class RookBulletScript : BulletScript {
	Collider2D[] enemies;
	float enemyToAttackDistance = 3.0f;
	float lastAttack = 0.0f;
	int enemyToAttackIndex = 0;
	float attackRadius = 10.0f;
	public LayerMask whatIsTargetable;
	Vector2 distanceVector;

	RookBulletScript() {
		//damage = 3.0f;
		//speed = 3.0f;
	}

	void Start() {
//		rigidbody2D.velocity *= speed;
		distanceVector = rigidbody2D.velocity;
		rigidbody2D.velocity = new Vector2 (0, 0);
		//Destroy (gameObject, 10);
		GameObject[] Towers = GameObject.FindGameObjectsWithTag("Tower");
		foreach(var tower in Towers) {
			Physics2D.IgnoreCollision(collider2D, tower.collider2D);
		}
		// Debug.Log ("distanceVector: " + distanceVector*speed);
        var r = gameObject.GetComponent<RectTransform>();
        r.pivot = new Vector2(0, r.rect.height / 2);
	    r.offsetMin = Vector2.zero;
        r.offsetMax = new Vector2(distanceVector.x, r.rect.height);
		r.rotation = new Quaternion(0, 0, (float)Math.Atan2(distanceVector.y, distanceVector.x), 1);
	}

	void Update() {
		transform.position = new Vector2 (transform.position.x + 1, transform.position.y);
	}


}