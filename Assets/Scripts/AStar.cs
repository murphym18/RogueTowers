using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar : MonoBehaviour {

	private GameObject target;
	private GameObject start;

	private List<Vector3> points = new List<Vector3>();

	// Use this for initialization
	void Awake () {
		start = this.gameObject;
		target = GetTarget();
		SetPathPoints();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GameObject GetTarget()
	{
		GameObject toTarget = GameObject.FindGameObjectWithTag ("Cage");
		if (toTarget == null)
		{
			// then cage was already destroyed
			toTarget = GameObject.FindGameObjectWithTag ("Player");
		}
		if (toTarget == null)
		{
			// then no target found, so return self
			return start;
		}

		return toTarget;
	}

	void SetPathPoints()
	{
		points.Clear();

		//points.Add(start.transform.position + new Vector3(10, -5, 0));
		points.Add(target.transform.position);
	}

	public List<Vector3> GetPoints()
	{
		return points;
	}
}
