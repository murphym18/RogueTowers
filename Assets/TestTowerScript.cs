using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestTowerScript : MonoBehaviour {
    public enum TowerType { Pawn, Knight, Bishop, Rook, King, Queen }

	GameObject target;
	Collider2D[] enemies;
	float enemyToAttackDistance = 10.0f;
	public Rigidbody2D Bullet;
	//public float bulletSpeed = 3.0f;
	float lastAttack = 0.0f;
	int enemyToAttackIndex = 0;

    // bool attackEnemy = false;
	public LayerMask whatIsTargetable;
	//public float damage = 1.0f;

    public float damageUpgradeMultiplier = 0.25f;
    public float speedUpgradeMultiplier = 0.2f;
    public float rangeUpgradeMultiplier = 0.25f;
	public float _attackDelay = 1.0f;

	public TowerType towerType;

    public static readonly Dictionary<TowerType, int> UpgradeLevels = new Dictionary<TowerType, int>();
    static TestTowerScript()
    {
        Initialize();
    }

    private static void Initialize()
    {
        foreach (TowerType towerType in Enum.GetValues(typeof (TowerType))) UpgradeLevels.Add(towerType, 0);
    }

    private float attackDelay
    {
        get { return _attackDelay / (1 + speedUpgradeMultiplier*UpgradeLevels[this.towerType]); }
        set { _attackDelay = value; }
    }

    public float _attackRadius = 10.0f;

    private float attackRadius
    {
        get { return _attackRadius*(1 + rangeUpgradeMultiplier*UpgradeLevels[towerType]); }
        set { _attackRadius = value; }
    }
//	private BulletType[] TowerInputKeys;
//	private class BulletType {
//		public string input;
//		public GameObject Bullet;
//	}


	// Use this for initialization
	void Start () {
		switch(towerType){
		case TowerType.Pawn :
			attackDelay = 1.0f;
			attackRadius = 10.0f;
			break;
		case TowerType.Knight :
			attackDelay = 2.5f;
			attackRadius = 10.0f;
			break;
		case TowerType.Bishop :
			attackDelay = .3f;
			attackRadius = 20.0f;
			break;
		case TowerType.Rook :
			attackDelay = 3.5f;
			attackRadius = 15.0f;
			break;
		case TowerType.King :
			attackDelay = 1.0f; //Have to see on how quickly the blast radius goes
			attackRadius = 5.0f;
			break;
		case TowerType.Queen :
			attackDelay = 3.0f;
			attackRadius = 50.0f;
			break;
			
		}



		//Destroy(gameObject, 0.7777777f);
//		BulletType[] = new BulletType[] {
//			createBulletType("ShootPawnBullet", PawnBullet),
//			createBulletType("ShootKnightBullet", KnightBullet),
//			createBulletType("ShootBishopnBullet", BishopBullet),
//			createBulletType("ShootRookBullet", RookBullet),
//			createBulletType("ShootKingnBullet", KingBullet),
//			createBulletType("ShootQueenBullet", QueenBullet)
//		};
	}//
	
	// Update is called once per frame
	void Update () {
	}
	void FixedUpdate(){
		//Physics2D.OverlapCircleNonAlloc(rigidbody2D.position, attackRadius, enemies, whatIsTargetable, 0, 0);
		enemies = Physics2D.OverlapCircleAll(rigidbody2D.position, attackRadius, whatIsTargetable, 0, 0);
		enemyToAttackIndex = -1;
		enemyToAttackDistance = attackRadius;
		for (int i = 0; enemies != null && i < enemies.Length; i++) {
			if((enemies[i].transform.position - this.transform.position).magnitude < enemyToAttackDistance){
				enemyToAttackIndex = i;
				enemyToAttackDistance = (enemies[i].transform.position - this.transform.position).magnitude;
			}
		}
		if (Time.time > lastAttack + attackDelay) {
			lastAttack = Time.time;

			if(enemyToAttackIndex != -1) {
                Rigidbody2D bulletInstance;

                bulletInstance = Instantiate(Bullet, transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                bulletInstance.velocity = (enemies[enemyToAttackIndex].transform.position - this.transform.position).normalized;
			    bulletInstance.GetComponent<BulletScript>().damage *= 1 + damageUpgradeMultiplier*UpgradeLevels[towerType];
			}

		}
	}
//	private BulletType createBulletType(string s, GameObject g) {
//		BulletType b = new BulletType ();
//		b.input = s;
//		b.Bullet = g;
//		return b;
//	}
}