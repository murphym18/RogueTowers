﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
	
	public float movementSpeed;
	public AStar aStar;
	public SpawnPoint spawnPoint;
	
	private GameObject target;
	private List<Vector3> points;
	private Vector3 nextPoint;
	private int pointIndex; // might need this
	private List<Vector3>.Enumerator pointEnumerator;
	private bool myActive = false;
	
	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag ("Cage");
		if (target == null)
			target = GameObject.FindGameObjectWithTag ("Player");

		pointIndex = 0;
	}

	void MyActivate()
	{
		if (spawnPoint == null)
		{
			gameObject.AddComponent("AStar");
			aStar = gameObject.GetComponent<AStar>();
			points = aStar.GetPoints();
		}
		else
		{
			points = spawnPoint.GetPathPoints();
			//Debug.Log("wegot points: " + points[0] + "," + points[1] + "," + points[2] + ",," + points[points.Count - 1]);
		}
		
		pointEnumerator = points.GetEnumerator();
		myActive = true;
	}

	// Update is called once per frame
	void Update () {
		if (!myActive)
			return;
		MoveSimple();
		//MoveDirect();
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