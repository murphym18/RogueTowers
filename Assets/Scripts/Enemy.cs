using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
	
	public float movementSpeed;
	
	private GameObject target;
	private AStar aStar;
	private List<Vector3> points;
	private Vector3 nextPoint;
	private int pointIndex; // might need this
	private List<Vector3>.Enumerator pointEnumerator;
	
	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag ("Cage");
		if (target == null)
			target = GameObject.FindGameObjectWithTag ("Player");
		aStar = gameObject.GetComponent<AStar>();
		points = aStar.GetPoints();
		pointEnumerator = points.GetEnumerator();

		pointIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
		//MoveSimple();
		MoveDirect();
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
		float minDist = 0.5f;
		return Vector3.Distance(this.rigidbody2D.position, nextPoint) < minDist;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Cage")
		{
			// stop and attack
			rigidbody2D.velocity = Vector3.zero;
			//Destroy(gameObject);
		}
	}
}
