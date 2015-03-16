using System;
using UnityEngine;
using System.Collections.Generic;

// The GameObject requires a RigidBody component

[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (BoxCollider2D))]
public class Tyrem : Character {
	
	public float maxSpeed = 5f;
	public float hp = 10f;

    public int upgradePoints { get; private set; }

	private const float invincibiltyTimeout = 0.8f;
	private const float knockbackTimeout = 0.3f;
	private float invincibiltyTimer = 0;
	private float knockbackTimer = 0;
	private float knockbackDistanceMulitplier = 5;
	private float maxHp;

    public void addUpgradePoints(int points)
    {
        upgradePoints += points;
    }

    public bool deductUpgradePoints(int points)
    {
        if (points > upgradePoints)
            return false;
        else
        {
            upgradePoints -= points;
            return true;
        }
    }
	
	// Use this for initialization
	void Start () {
		maxHp = hp;
	}
	
	// Update is called once per frame
	void Update () {
		if (invincibiltyTimer > 0)
			invincibiltyTimer -= Time.deltaTime;
		if (knockbackTimer > 0) // lock movement when getting knockedback
			knockbackTimer -= Time.deltaTime;
		else
		{
			float h = Input.GetAxis ("MovePlayerHorizontal");
			float v = Input.GetAxis ("MovePlayerVertical");
			rigidbody2D.velocity = (new Vector2 (h, v)).normalized * maxSpeed;
		}
	}

	public void damage(float power)
	{
		hp -= power;
		//if (hp <=0)
			// Debug.Log("Player health is 0");
	}

	// Collissions
	void OnCollisionEnter2D(Collision2D coll)
	{
		if (invincibiltyTimer <= 0)
		{
			if (coll.gameObject.tag == "Enemy")
			{
				Vector3 knockback = (rigidbody2D.position - coll.gameObject.rigidbody2D.position) * knockbackDistanceMulitplier;
				rigidbody2D.velocity = knockback;
				invincibiltyTimer = invincibiltyTimeout;
				knockbackTimer = knockbackTimeout;
				damage(coll.gameObject.GetComponentInParent<Enemy>().attackPower);
			}
		}
	}

	public float getMaxHp() {
		return maxHp;
	}
}
