using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestTowerScript : MonoBehaviour {
    public enum TowerType { Pawn, Knight, Bishop, Rook, King, Queen }
    public enum TargetPriority { Closest, Furthest }

	GameObject target;
	Collider2D[] enemies;
	float enemyToAttackDistance = 10.0f;
    public GameObject Bullet;
    public float bulletYOffset = 0f;
	float lastAttack = 0.0f;
	int enemyToAttackIndex = 0;

	public LayerMask whatIsTargetable;

    public float damageUpgradeMultiplier = 0.25f;
    public float speedUpgradeMultiplier = 0.2f;
    public float rangeUpgradeMultiplier = 0.25f;
	public float _attackDelay = 1.0f;

	public TowerType towerType;
    public TargetPriority targetPriority = TargetPriority.Closest;

    public static readonly Dictionary<TowerType, int> UpgradeLevels = new Dictionary<TowerType, int>();

    public static void Initialize()
    {
        foreach (TowerType towerType in Enum.GetValues(typeof (TowerType))) UpgradeLevels[towerType] = 0;
    }

    private float attackDelay
    {
        get { return _attackDelay / (1 + speedUpgradeMultiplier*UpgradeLevels[this.towerType]); }
        set { _attackDelay = value; }
    }

    public float _attackRadius = 10.0f;

    public float attackRadius
    {
        get { return _attackRadius*(1 + rangeUpgradeMultiplier*UpgradeLevels[towerType]); }
        set { _attackRadius = value; }
    }
	
	// Update is called once per frame
    private void Update()
    {
        //Physics2D.OverlapCircleNonAlloc(rigidbody2D.position, attackRadius, enemies, whatIsTargetable, 0, 0);
        enemies = Physics2D.OverlapCircleAll(rigidbody2D.position, attackRadius, whatIsTargetable, 0, 0);
        enemyToAttackIndex = TargetingPriority(this.transform, enemies);
        if (Time.time > lastAttack + attackDelay)
        {
            lastAttack = Time.time;

            if (enemyToAttackIndex != -1)
            {
                GameObject bulletInstance;

                bulletInstance =
                    Instantiate(Bullet, transform.position + new Vector3(0, bulletYOffset), Quaternion.identity) as GameObject;
                bulletInstance.GetComponent<BulletScript>().velocity =
                    enemies[enemyToAttackIndex].transform.position - this.transform.position;
                bulletInstance.GetComponent<BulletScript>().damage *=
                    1 + damageUpgradeMultiplier*UpgradeLevels[towerType];
            }

        }
    }

    void FixedUpdate(){
	}

    int TargetingPriority(Transform tower, Collider2D[] enemies)
    {
        switch (this.targetPriority)
        {
            case TargetPriority.Closest:
                return Closest(tower, enemies);
            case TargetPriority.Furthest:
                return Furthest(tower, enemies);

            default:
                return Closest(tower, enemies);
        }
    }

    static int Closest(Transform tower, Collider2D[] enemies)
    {
        var idx = -1;
        var dist = float.MaxValue;
        for (int i = 0; enemies != null && i < enemies.Length; i++)
        {
            if ((enemies[i].transform.position - tower.position).magnitude < dist)
            {
                idx = i;
                dist = (enemies[i].transform.position - tower.position).magnitude;
            }
        }
        return idx;
    }

    static int Furthest(Transform tower, Collider2D[] enemies)
    {
        var idx = -1;
        var dist = float.MinValue;
        for (int i = 0; enemies != null && i < enemies.Length; i++)
        {
            if ((enemies[i].transform.position - tower.position).magnitude > dist)
            {
                idx = i;
                dist = (enemies[i].transform.position - tower.position).magnitude;
            }
        }
        return idx;
    }
//	private BulletType createBulletType(string s, GameObject g) {
//		BulletType b = new BulletType ();
//		b.input = s;
//		b.Bullet = g;
//		return b;
//	}
}