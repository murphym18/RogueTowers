using UnityEngine;
using System.Collections;
using System;

public class HealthBarScript : MonoBehaviour {

	public GameObject gameManager;
	Tyrem owner;
	float health { get { return owner.hp; } }

	// Use this for initialization
	void Start () {
		owner = gameManager.GetComponent<GameManager> ().PlayerInstance.GetComponent<Tyrem>();
	}	
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3 (Math.Max(health, 0), 1, 1); 
		transform.position = new Vector3 (owner.transform.position.x, owner.transform.position.y - 0.5f, 0f);
		//Debug.Log (health);
	}
}
