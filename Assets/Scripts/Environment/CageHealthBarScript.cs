using UnityEngine;
using System.Collections;
using System;

public class CageHealthBarScript : MonoBehaviour {

	public GameObject gameManager;
	GameObject[] cages;
	GameObject currentCage;

	float health { get { return currentCage.GetComponent<Cage>().hp; } }
	int currentLevel { get { return gameManager.GetComponent<GameManager>().currentLevel; } }

	// Use this for initialization
	void Start () {
		//owner = gameManager.GetComponent<GameManager> ().PlayerInstance;
		cages = gameManager.GetComponent<BoardManager> ().levelCages;
		currentCage = cages [currentLevel]; 
	}	
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3 (Math.Max(health, 0), 1, 1); 
		transform.position = new Vector3 (currentCage.transform.position.x, currentCage.transform.position.y - 0.5f, 0f);
		//Debug.Log (health);
	}
}
