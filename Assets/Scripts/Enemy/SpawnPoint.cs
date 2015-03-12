using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPoint : IsometricObject {

	public GameObject[] enemySpawns;
	public int spawnInterval = 3;
	public int nextWaveLevel = 1;

	private GameObject target;
	private List<Vector3> pathPoints = null;
	private int level = 0;

	private GameManager gameManager;
	private BoardManager boardManager;
	private AStar aStarScript;

	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		boardManager = gameManager.GetComponentInParent<BoardManager>();
		aStarScript = gameManager.GetComponentInParent<AStar>();
	}

	// Initializes a spawnpoint with a target and an initial path to target
	public void Initialize(int level)
	{
		this.level = level;
		target = GetInitialTarget();
		pathPoints = new List<Vector3>(GetInitialPath());
	}

	// Called by WaveManagerScript
	// Starts a coroutine to spawn enemies at given interval.
	public void SendNextWave()
	{
		StartCoroutine(SpawnAtInterval(spawnInterval, nextWaveLevel++));
	}

	// Spawns the enemySpawns list of enemies, waveLevel times (enemySpawns * waveLevel).
	// Waits the given seconds between enemy spawns.
	private IEnumerator SpawnAtInterval(int seconds, int waveLevel)
	{
		int maxEnemies = enemySpawns.Length * waveLevel;
		int enemyCount = 0;
		
		while (enemyCount < maxEnemies)
		{
			foreach (GameObject enemyType in enemySpawns)
			{
				if (enemyCount++ > maxEnemies)
					break;

				// Checks if spawnPoint is enabled, should not be neccessary, need to test.
				if (enabled)
					SpawnEnemy(enemyType);
				
				yield return new WaitForSeconds(seconds);
			}
		}
	}

	private void SpawnEnemy(GameObject enemyType)
	{
		GameObject newEnemy = (GameObject)Instantiate (enemyType, transform.position, Quaternion.identity);
		newEnemy.GetComponent<Enemy>().Initialize(this, level);
		newEnemy.GetComponent<Enemy>().enabled = true;
	}

	// Give the spawnpoint a new target. i.e. cage to player
	public void SetTarget(GameObject newTarget)
	{
		this.target = newTarget;
		RequestNewPath();
	}

	// Called by an Enemy to get path to target.
	public List<Vector3> GetPathPoints()
	{
		return pathPoints;
	}

	// Called by an Enemy to initialize with a target.
	public GameObject GetTarget()
	{
		return target;
	}

	// Sends a path calculation job to AStar.
	private void RequestNewPath()
	{
		aStarScript.AStarEnqueue(this.transform.position, this.gameObject);
	}

	// Called by AStar when a new path is calculated.
	// Set the new path and reset the pointIndex to 0.
	public void SetPathPointsCallback(List<Vector3> newPath)
	{
		pathPoints = new List<Vector3>(newPath);
	}

	// Gets a target if it exists. Cage -> Player -> Self.
	private GameObject GetInitialTarget()
	{
		GameObject initialTarget = boardManager.levelCages[gameManager.currentLevel];
		if (initialTarget == null)
			initialTarget = gameManager.PlayerInstance;
		if (initialTarget == null)
			initialTarget = this.gameObject;
		
		return initialTarget;
	}
	
	// Calculates an initial path to AStar's target
	private List<Vector3> GetInitialPath()
	{
		aStarScript.CalculateAStar(transform.position);
		return aStarScript.GetPoints();
	}

}
