﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (BoxCollider2D))]
public class Enemy : IsometricObject {

	public float health = 10;
	public float movementSpeed = 1;
	public float attackPower = 1;
	public float attackRate = 1;
	public float level = 1;

	public float healthMultiplier = 0.20f;
	public float movementSpeedMultiplier = 0.08f;
	public float attackPowerMultiplier = 0.12f;
	public float attackRateMultiplier = 0.15f;


	public float spawnFrequency = 1.0f;
	public enum EnemyType {Basic, Fast, Strong}
	public EnemyType enemyType;

	Animator anim;
	public int direction;
	public enum Direction { Front, Back, Left, Right}

	public Color damagedColor = new Color(0.8F,0.6F,0.6F,0.9F);
	
	private GameObject target;
	private GameObject player;
	private SpawnPoint spawnPoint;
	private int pointIndex = 0;
	private Vector3 nextPoint;
	private List<Vector3> points = null;
	private List<Vector3>.Enumerator pointEnumerator;
	private RaycastHit2D[] oneArray = new RaycastHit2D[1];
	private const float attackTimeout = 1;
	private float attackTimer = 0;
	private float damagedColorTimeout = 0.2f;
	private float damagedColorTimer = 0;

	private bool attackingPlayer = false;
	private const float attackTellTimeout = 0.2f;
	private float attackTellTimer = 0;
	private const float attackPlayerTimeout = 1.0f;
	private float attackPlayerTimer = 0;
	private Vector3 attackTarget;
	private Vector3 attackTargetVelocity;

	private GameManager gameManager;
	private BoardManager boardManager;
	private AStar aStarScript;
	private BoxCollider2D boxCollider;
	private SpriteRenderer spriteRenderer;
	private LayerMask obstructionMask;

	void Awake()
	{
		anim = GetComponent<Animator> ();
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		boardManager = gameManager.GetComponentInParent<BoardManager>();
		aStarScript = gameManager.GetComponentInParent<AStar>();
		boxCollider = GetComponent<BoxCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		player = gameManager.playerInstance;

		obstructionMask = LayerMask.GetMask("Walls", "Towers", "Chests");
	}

	public void Initialize(SpawnPoint givenSpawnPoint, int level)
	{
		SetLevel(level);
		Initialize(givenSpawnPoint);
	}

	// Initializes a spawnpoint by giving it a target and a path.
	// If enemy came from a spawnpoint, take spawnpoint's target and path.
	// Otherwise, get a target and form a path.
	public void Initialize(SpawnPoint givenSpawnPoint)
	{
		spawnPoint = givenSpawnPoint;

		if (spawnPoint != null)
		{
			target = spawnPoint.GetTarget();
			points = spawnPoint.GetPathPoints();
		}
		else
		{
			target = GetInitialTarget();
			points = new List<Vector3>(GetInitialPath());
		}
		
		pointEnumerator = points.GetEnumerator();
	}

	// If enemy can see target, just move to the target
	// otherwise, follow the path points.
	void FixedUpdate()
	{
		UpdateColors();
		UpdateDirection();

		if (attackTimer > 0)
			attackTimer -= Time.fixedDeltaTime;
		else if (attackingPlayer || PlayerInAttackRange())
			AttackPlayer();
		else if (CanSeePoint(target.transform.position))
			MoveDirectlyToTarget();
		else if (points != null)
			MoveToFollowPath();
	}

	void UpdateColors()
	{
		if (damagedColorTimer > 0)
		{
			if ((damagedColorTimer -= Time.fixedDeltaTime) < 0)
				spriteRenderer.material.color = Color.white;
		}
	}

	// Give the enemy a new target. i.e. cage to player
	public void SetTarget(GameObject newTarget)
	{
		this.target = newTarget;
	}

	public void KillSelf(float time)
	{
		WaveManagerScript.enemiesRemaining[enemyType]--;
		WaveManagerScript.UpdateWavePanel();
		Destroy(gameObject, time);
	}

	// Collissions
	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Cage")
		{
			if (!coll.gameObject.GetComponentInParent<CageScript>().isDestroyed
			    && !coll.gameObject.GetComponentInParent<CageScript>().isUnlocked)
			{
				// stop and attack
				rigidbody2D.velocity = Vector3.zero;
				coll.gameObject.GetComponentInParent<CageScript>().damage();
				KillSelf(0.0f);
			}
		}
		else if (coll.gameObject.tag == "Player")
		{
			rigidbody2D.velocity = Vector3.zero;
			attackTimer = attackTimeout;
		}
	}

	// Triggers
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Bullet")
		{
			float bulletDamage = coll.GetComponentInParent<BulletScript>().damage;
			this.health -= bulletDamage;

			damagedColorTimer = damagedColorTimeout;
			spriteRenderer.material.color = damagedColor;

			if (this.health <= 0)
				KillSelf(0.0f);
		}
	}

	void SetLevel(int level)
	{
		this.level = level;
		health *= 1 + level*healthMultiplier;
		movementSpeed *= 1 + level*movementSpeedMultiplier;
		attackPower *= 1 + level*attackPowerMultiplier;
		attackRate *= 1 + level*attackRateMultiplier;
	}

	// Follows the pathpoints.
	// if the following point is in sight, skips the current point.
	private void MoveToFollowPath()
	{
		if (IsAtPoint(nextPoint) || pointIndex == 0)
		{
			if (pointEnumerator.MoveNext())
			{
				pointIndex++;
				while (pointIndex < points.Count && CanSeePoint(points[pointIndex]))
				{
					pointIndex++;
					pointEnumerator.MoveNext();
				}
				nextPoint = pointEnumerator.Current;
			}
		}
		MoveToNextPoint();
	}

	// move directly towards the target
	private void MoveDirectlyToTarget() {
		Vector3 direction = target.collider2D.transform.position - (Vector3)this.rigidbody2D.position;
		Vector3 velocity = direction.normalized * movementSpeed;
		//rigidbody2D.AddForce(velocity);
		rigidbody2D.velocity = velocity;
	}
	
	// move to the next path point
	private void MoveToNextPoint()
	{
		//Vector3 direction = nextPoint - this.transform.position;
		Vector3 direction = nextPoint - new Vector3(this.rigidbody2D.position.x, this.rigidbody2D.position.y);
		Vector3 velocity = direction.normalized * movementSpeed;
		//rigidbody2D.AddForce(velocity);
		rigidbody2D.velocity = velocity;
	}
	
	// Checks if at the next path point
	private bool IsAtPoint(Vector3 point)
	{
		float minDist = 0.1f;
		return Vector3.Distance(this.rigidbody2D.position, point) < minDist;
	}

	private bool PlayerInAttackRange()
	{
		float attackRange = 2.5f;
		float dist = (player.collider2D.transform.position - this.collider2D.transform.position).sqrMagnitude;
		return (dist < (attackRange*attackRange) && CanSeePoint(player.collider2D.transform.position));
	}

	private void AttackPlayer()
	{
		if (!attackingPlayer)
		{
			if (attackTellTimer <= 0)
			{
				attackTellTimer = attackTellTimeout;
				rigidbody2D.velocity = Vector3.zero;
			}
			else
			{
				if ((attackTellTimer -= Time.fixedDeltaTime) <= 0)
				{
					attackTarget = player.collider2D.transform.position;
					attackTargetVelocity = (attackTarget - (Vector3)this.rigidbody2D.position).normalized * (movementSpeed * 1.9f);
					attackingPlayer = true;
					attackPlayerTimer = attackPlayerTimeout;
				}
			}
		}
		else if (attackPlayerTimer > 0 && !IsAtPoint(attackTarget))
		{
			rigidbody2D.velocity = attackTargetVelocity;
			attackPlayerTimer -= Time.fixedDeltaTime;
		}
		else
		{
			rigidbody2D.velocity = Vector3.zero;
			attackingPlayer = false;
			RequestNewPath();
		}
	}

	// Checks if has unobstructed direct path line to the given point
	private bool CanSeePoint(Vector3 point)
	{
		Vector2 start = new Vector2(this.transform.position.x, this.transform.position.y);
		Vector2 dest = new Vector2(point.x, point.y);
		//float thickness = 0.32f;
		float thickness = boxCollider.size.x / 2 + 0.05f;
		Physics2D.raycastsStartInColliders = true;
		int hit1, hit2;
		
		if ((dest.x < start.x && dest.y > start.y) || (dest.x > start.x && dest.y < start.y))
		{
			hit1 = Physics2D.LinecastNonAlloc(new Vector2(start.x-thickness, start.y-thickness), new Vector2(dest.x, dest.y), oneArray, obstructionMask.value);
			hit2 = Physics2D.LinecastNonAlloc(new Vector2(start.x+thickness, start.y+thickness), new Vector2(dest.x, dest.y), oneArray, obstructionMask.value);
		}
		else if ((dest.x < start.x && dest.y < start.y) || (dest.x > start.x && dest.y > start.y))
		{
			hit1 = Physics2D.LinecastNonAlloc(new Vector2(start.x-thickness, start.y+thickness), new Vector2(dest.x, dest.y), oneArray, obstructionMask.value);
			hit2 = Physics2D.LinecastNonAlloc(new Vector2(start.x+thickness, start.y-thickness), new Vector2(dest.x, dest.y), oneArray, obstructionMask.value);
		}
		else
		{
			hit1 = hit2 = Physics2D.LinecastNonAlloc(new Vector2(start.x, start.y), new Vector2(dest.x, dest.y), oneArray, obstructionMask.value);
		}
		
		return hit1 == 0 && hit2 == 0;
	}
	
	// Called by AStar when AStar sees the target move. Just request a new path.
	// Astar will callback with SetPathPointsCallback when the new path is calculated.
	public void TellTargetMoved()
	{
		RequestNewPath();
	}

	// Sends a path calculation job to AStar.
	private void RequestNewPath()
	{
		aStarScript.AStarEnqueue(this.transform.position, this.gameObject);
	}

	// Called by AStar when a new path is calculated.
	// Set the new path and reset the pointIndex to 0.
	public void SetPathPointsCallback(List<Vector3> newPath)
	{
		if (newPath.Count > 0)
		{
			points = new List<Vector3>(newPath);
			pointEnumerator = points.GetEnumerator();
			pointIndex = 0;
		}
	}

	// Calculates an initial path to AStar's target
	private List<Vector3> GetInitialPath()
	{
		aStarScript.CalculateAStar(this.transform.position);
		return aStarScript.GetPoints();
	}

	// Gets a target if it exists. Cage -> Player -> Self.
	private GameObject GetInitialTarget()
	{
		GameObject initialTarget = boardManager.levelCages[gameManager.currentLevel];
		if (initialTarget == null)
			initialTarget = gameManager.playerInstance;
		if (initialTarget == null)
			initialTarget = this.gameObject;
		
		return initialTarget;
	}
	private void UpdateDirection() {
		var velocityX = Mathf.Abs(rigidbody2D.velocity.x);
		var velocityY = Mathf.Abs(rigidbody2D.velocity.y);

		if(velocityX >= velocityY && rigidbody2D.velocity.x > 0) {
			direction = 3;
		} else if(velocityX >= velocityY && rigidbody2D.velocity.x < 0) {
			direction = 2;
		}else if(velocityX <= velocityY && rigidbody2D.velocity.y < 0) {
			direction = 0;
		} else if(velocityX <= velocityY && rigidbody2D.velocity.y > 0){
			direction = 1;
		}
		anim.SetInteger ("Direction", direction);
	}

}
