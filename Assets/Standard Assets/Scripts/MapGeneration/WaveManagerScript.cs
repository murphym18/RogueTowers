using UnityEngine;
using System.Collections;

public class WaveManagerScript : MonoBehaviour {

	private GameObject[] spawnPoints;

	// Use this for initialization
	void Start () {
		spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
	}
	
	// Update is called once per frame
	void Update () {
		spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
		if (enemy == null)
		{
			foreach(GameObject spawnPoint in spawnPoints)
			{
				spawnPoint.SendMessage("sendNextWave");
			}
		}
	}
}
