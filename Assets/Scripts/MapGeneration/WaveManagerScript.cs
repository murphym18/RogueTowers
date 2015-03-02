using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManagerScript : MonoBehaviour {

	private List<GameObject> spawnPoints = new List<GameObject>();
	private bool inCooldown = true;
	public int cooldownTime = 2;
	
	// Finds all spawnpoints then immediately sends a wave
	public void InitializeWaves()
	{
		spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("SpawnPoint"));
		inCooldown = false;
		sendWave ();
	}

	// If in cooldown, do nothing
	// if not in cooldown, check if enemies all gone,
	//		if they are, cooldown then send next wave
	void Update () {
		if (!inCooldown)
		{
			GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
			if (enemy == null)
			{
				inCooldown = true;
				StartCoroutine( waitForCooldown(cooldownTime) );
			}
		}
	}

	// add a new spawnpoint to the list
	public void AddToWaveManagerList(GameObject newSpawnPoint)
	{
		spawnPoints.Add(newSpawnPoint);
	}

	// Waits the cooldown period, then sends next wave
	IEnumerator waitForCooldown(int seconds)
	{
		yield return new WaitForSeconds(seconds);
		sendWave ();
		inCooldown = false;
	}

	// Tells each spawnPoint to call the function sendNextWave
	void sendWave()
	{
		foreach(GameObject spawnPoint in spawnPoints)
		{
			spawnPoint.GetComponent<SpawnPoint>().sendNextWave();
		}
	}
}
