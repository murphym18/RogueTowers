﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManagerScript : MonoBehaviour {
	
	public int cooldownTime = 2;

	private List<GameObject> curLevelSpawnPoints = new List<GameObject>();
	private bool inCooldown = true;
	private GameManager gameManager;
	private BoardManager boardManager;
	private GameObject cage; //TODO: use somehow
	private GameObject player;
	private int curLevel = -1;
	private bool stopWave = false;

	void Awake()
	{
		gameManager = this.GetComponent<GameManager>();
		boardManager = gameManager.GetComponent<BoardManager>();
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

	// Waits the cooldown period, then sends next wave
	IEnumerator waitForCooldown(int seconds)
	{
		yield return new WaitForSeconds(seconds);
		if (!stopWave)
		{
			sendWave ();
			inCooldown = false;
		}
		else
		{
			stopWave = false;
		}
	}

	// Tells each spawnPoint to call the function sendNextWave
	void sendWave()
	{
		foreach(GameObject spawnPoint in curLevelSpawnPoints)
		{
			spawnPoint.GetComponent<SpawnPoint>().sendNextWave();
		}
	}


	void InitializeSpawnPoints(int level)
	{
		curLevelSpawnPoints = boardManager.levelSpawnPoints[level];
		foreach (GameObject spawnPoint in boardManager.levelSpawnPoints[level])
		{
			spawnPoint.GetComponent<SpawnPoint>().Initialize(level);
		}
	}

	public void RecalculatePaths()
	{
		int level = gameManager.currentLevel;
		curLevelSpawnPoints = boardManager.levelSpawnPoints[level];
		foreach (GameObject spawnPoint in boardManager.levelSpawnPoints[level])
		{
			spawnPoint.GetComponent<SpawnPoint>().Initialize(level);
		}
	}

	// When players steps on the trigger tile for the next level
	public void TriggerNextLevel(int nextLevel)
	{
		// Start next level
		if (nextLevel > curLevel && nextLevel < boardManager.numLevels)
		{
			GetComponent<AStar>().PrintNodeMesh(0);

			gameManager.currentLevel = nextLevel;
			curLevel = nextLevel;
			InitializeSpawnPoints(nextLevel);
			inCooldown = false;
			sendWave();
		}
	}

	public void TriggerCageDestroyed()
	{
		stopWave = true;
		inCooldown = true;
		curLevelSpawnPoints = new List<GameObject>();
		Debug.Log("Cage Destroyed!");
	}

	public void TriggerCageUnlocked()
	{
		stopWave = true;
		inCooldown = true;
		curLevelSpawnPoints = new List<GameObject>();
		Debug.Log("Cage Unlocked!");
	}
}
