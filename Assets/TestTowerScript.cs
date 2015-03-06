using UnityEngine;
using System.Collections;
public class TestTowerScript : MonoBehaviour {
	// bool attackEnemy = false;
	public float attackRadius = 10.0f;
	public LayerMask whatIsTargetable;
	public float damage = 1.0f;
	public float attackDelay = 1.0f;

	GameObject target;
	Collider2D[] enemies;
	float enemyToAttack = 10.0f;
	public Rigidbody2D Bullet;
	public float bulletSpeed = 3.0f;
	float lastAttack = 0.0f;
	int enemyToAttackIndex = 0;
	// Use this for initialization
	void Start () {
		//Destroy(gameObject, 0.7777777f);
	}//
	
	// Update is called once per frame
	void Update () {
	}
	void FixedUpdate(){
		//Physics2D.OverlapCircleNonAlloc(rigidbody2D.position, attackRadius, enemies, whatIsTargetable, 0, 0);
		enemies = Physics2D.OverlapCircleAll(rigidbody2D.position, attackRadius, whatIsTargetable, 0, 0);
		enemyToAttackIndex = -1;
		enemyToAttack = attackRadius;
		for (int i = 0; enemies != null && i < enemies.Length; i++) {
			if((enemies[i].transform.position - this.transform.position).magnitude < enemyToAttack){
				enemyToAttackIndex = i;
				enemyToAttack = (enemies[i].transform.position - this.transform.position).magnitude;
			}
		}
		if (Time.time > lastAttack + attackDelay) {
			lastAttack = Time.time;

			if(enemyToAttackIndex != -1) {
				Rigidbody2D bulletInstance = Instantiate (Bullet, transform.position, Quaternion.Euler (new Vector3 (0, 0, 0))) as Rigidbody2D;
				bulletInstance.GetComponentInParent<BulletScript>().damage = damage;
				bulletInstance.velocity = (enemies[enemyToAttackIndex].transform.position - this.transform.position).normalized*bulletSpeed;
			}

		}
	}
}