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
		damage = 3.0f;
		speed = 3.0f;
	}

	void Start() {
//		rigidbody2D.velocity *= speed;
		distanceVector = rigidbody2D.velocity;
		rigidbody2D.velocity = new Vector2 (0, 0);
		//Destroy (gameObject, 10);
		GameObject[] Towers = GameObject.FindGameObjectsWithTag("Tower");
		for(int i = 0; Towers != null && i < Towers.Length; i++) {
			Physics2D.IgnoreCollision(collider2D, Towers[i].collider2D);
		}
		Debug.Log ("distanceVector: " + distanceVector*speed);
		gameObject.GetComponent<RectTransform> ().sizeDelta = distanceVector*speed*100;
		gameObject.GetComponent<RectTransform> ().rotation = new Quaternion(distanceVector.x, distanceVector.y, 0, 1);
			
	}

	void Update() {
		transform.position = new Vector2 (transform.position.x + 1, transform.position.y);
	}




}