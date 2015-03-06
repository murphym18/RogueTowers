using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

	public float health = 10;
	public float movementSpeed = 1;
	public float attackPower = 1;
	public float attackRate = 1;
	public float level = 1;

	private AStar aStarScript;
	private GameObject target;
	private SpawnPoint spawnPoint;
	private List<Vector3> points;
	private Vector3 nextPoint;
	private int pointIndex = 0; // might need this
	private List<Vector3>.Enumerator pointEnumerator;
	private float lastPathUpdate = 0f;
	private LayerMask obstructionMask;

	private GameObject gameManager;
	private BoardManager boardManager;
	private int curLevel;

	void Awake()
	{
		gameManager = GameObject.Find("GameManager");
		boardManager = gameManager.GetComponent<BoardManager>();
		curLevel = gameManager.GetComponent<GameManager>().currentLevel;

		target = boardManager.levelCages[curLevel];
		if (target == null)
			target = GameObject.FindGameObjectWithTag ("Player");

		obstructionMask = LayerMask.GetMask("Default");
	}

	public void Initialize(SpawnPoint spawnPoint)
	{
		this.spawnPoint = spawnPoint;
		if (this.spawnPoint == null)
		{
			Debug.Log("activating without spawn point");
			aStarScript = gameObject.GetComponent<AStar>();
			aStarScript.Initialize();
			points = aStarScript.GetPoints();
		}
		else
		{
			//Debug.Log("activating with spawn point");
			points = spawnPoint.GetPathPoints();
		}
		
		pointEnumerator = points.GetEnumerator();
	}
	
	void Update () {
		/*
		Vector2 toTarget = new Vector2(target.transform.position.x - this.transform.position.x, target.transform.position.y - this.transform.position.y);
		float distanceToTarget = toTarget.magnitude;

		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, toTarget.normalized, distanceToTarget);
		if (hit.collider == null) {
			MoveDirect();
			return;
		}
		*/

		MoveSimple();
	}
	
	void MoveSimple()
	{
		if (IsAtNextPoint() || pointIndex == 0)
		{
			if (pointEnumerator.MoveNext())
			{
				pointIndex++;
				nextPoint = pointEnumerator.Current;
			}
			else
			{
				// reached end of path
			}
		}

		MoveToNextPoint();
	}

	void MoveDirect() {
		Vector3 direction = target.transform.rigidbody2D.position - this.rigidbody2D.position;
		Vector3 velocity = direction.normalized * movementSpeed;
		rigidbody2D.velocity = velocity;
	}

	void MoveToNextPoint()
	{
		Vector3 direction = nextPoint - this.transform.position;
		Vector3 velocity = direction.normalized * movementSpeed;
		rigidbody2D.velocity = velocity;
	}

	bool IsAtNextPoint()
	{
		float minDist = 0.3f;
		return Vector3.Distance(this.rigidbody2D.position, nextPoint) < minDist;
	}

	public void SetAStarScript(AStar astar)
	{
		aStarScript = astar;
	}

	public void SetSpawnPoint(SpawnPoint parent)
	{
		spawnPoint = parent;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Cage")
		{
			// stop and attack
			rigidbody2D.velocity = Vector3.zero;
			Destroy(gameObject, 2.0f);
		}

	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Bullet")
		{
			// stop and attack
			float bulletDamage = coll.GetComponentInParent<BulletScript>().damage;
			this.health -= bulletDamage;
			if (this.health <= 0)
				Destroy(gameObject, 0.0f);
		}
	}
}
