using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
	
	public float movementSpeed;

	private AStar aStar;
	private GameObject target;
	private SpawnPoint spawnPoint;
	private List<Vector3> points;
	private Vector3 nextPoint;
	private int pointIndex = 0; // might need this
	private List<Vector3>.Enumerator pointEnumerator;

	void Awake()
	{
		target = GameObject.FindGameObjectWithTag ("Cage");
		if (target == null)
			target = GameObject.FindGameObjectWithTag ("Player");
	}

	public void Initialize(SpawnPoint spawnPoint)
	{
		this.spawnPoint = spawnPoint;
		if (spawnPoint == null)
		{
			Debug.Log("activating without spawn point");
			aStar = gameObject.GetComponent<AStar>();
			points = aStar.GetPoints();
		}
		else
		{
			//Debug.Log("activating with spawn point");
			points = spawnPoint.GetPathPoints();
		}
		
		pointEnumerator = points.GetEnumerator();
	}
	
	void Update () {
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
		aStar = astar;
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
			
			Destroy(gameObject, 0.0f);
		}
	}
}
