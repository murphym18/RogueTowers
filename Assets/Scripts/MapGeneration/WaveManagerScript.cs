using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManagerScript : MonoBehaviour {
	
	public int cooldownTime = 2;
	public float enemyCountMultiplier = 1.5f;
	public float nextWaveMultiplier = 0.3f;
	public float nextLevelMultiplier = 0.3f;
	public List<GameObject> enemyList;
	public static Dictionary<Enemy.EnemyType, int> enemiesRemaining = new Dictionary<Enemy.EnemyType, int>();
	public GameObject wavePanelObject;

	public int waveNumber = 0;
	private int curLevel = -1;
	private bool inCooldown = true;
	private bool stopWave = false;
	private bool inCooldownTimer = false;
	private bool levelComplete = false;
	private List<GameObject> curLevelSpawnPoints = new List<GameObject>();
	public List<GameObject> nextWaveList = new List<GameObject>();
	
	private GameManager gameManager;
	private BoardManager boardManager;
	private AStar aStarScript;
	private GameObject player;
	private static WavePanelScript wavePanelScript;

	void Awake()
	{
		gameManager = this.GetComponent<GameManager>();
		boardManager = this.GetComponent<BoardManager>();
		aStarScript = this.GetComponent<AStar>();
	}

	void Start()
	{
		player = gameManager.playerInstance;
		for (int level = 0; level < boardManager.numLevels; level++)
		{
			GetComponent<AStar>().Initialize(level);
		}
		enemiesRemaining.Clear();
		foreach (Enemy.EnemyType type in Enum.GetValues( typeof(Enemy.EnemyType)))
			enemiesRemaining.Add(type, 0);

		wavePanelScript = wavePanelObject.GetComponent<WavePanelScript>();
	}

	// If in cooldown, do nothing
	// if not in cooldown, check if enemies all gone,
	//		if they are, cooldown then send next wave
	void Update () {
		if (!inCooldown)
		{
			if (EnemiesCleared())
			{
				gameManager.DisplayMessage("wave cleared");
				inCooldown = true;
				StartCoroutine( waitForCooldown(cooldownTime) );
			}
		}
		else if (levelComplete)
		{
			if (EnemiesCleared())
			{
				gameManager.DisplayMessage("level complete");
				ToggleBorderTiles(false, curLevel);
				player.GetComponent<TowerPlacement>().RecallAllTowers();
				levelComplete = false;
			}
		}
	}

	public static void UpdateWavePanel()
	{
		wavePanelScript.UpdatePanel();
	}

	private bool EnemiesCleared()
	{
		GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
		return enemy == null;
	}

	private void ToggleBorderTiles(bool isEnabled, int level)
	{
		foreach (GameObject borderTile in boardManager.levelBorderTiles[level])
		{
			borderTile.SetActive(isEnabled);
		}
	}

	// Waits the cooldown period, then sends next wave
	IEnumerator waitForCooldown(int seconds)
	{
		inCooldownTimer = true;
		yield return new WaitForSeconds(seconds);
		if (!stopWave)
		{
			sendWave ();
			inCooldown = false;
		}
		else
		{
			ToggleBorderTiles(false, curLevel);
			levelComplete = false;
			stopWave = false;
		}
		inCooldownTimer = false;
	}

	public int curEnemyCount
	{
		get { 
			int remaining = 0;
			foreach (KeyValuePair<Enemy.EnemyType, int> pair in enemiesRemaining)
				remaining += pair.Value;
			return remaining;
		}
	}

	private float finalEnemyCountMultiplier
	{
		get { return enemyCountMultiplier +
					waveNumber * nextWaveMultiplier +
					curLevel * nextLevelMultiplier; }
	}

	void CreateNextWaveList()
	{
		nextWaveList.Clear();
		foreach(GameObject enemy in enemyList)
		{
			for (int i = 0; i < Mathf.FloorToInt( enemy.GetComponent<Enemy>().spawnFrequency * finalEnemyCountMultiplier); i++)
			{
				nextWaveList.Add(enemy);
				enemiesRemaining[enemy.GetComponent<Enemy>().enemyType]++;
			}
		}
	}

	// Tells each spawnPoint to call the function SendNextWave
	void sendWave()
	{
		gameManager.DisplayMessage("Wave Incoming");
		waveNumber++;
		CreateNextWaveList();
		nextWaveList = new List<GameObject>( Helpers.EnumerableFuncs.toShuffledArray(nextWaveList) );

		for (int i = 0; i < curLevelSpawnPoints.Count; i++)
		{
			curLevelSpawnPoints[i].GetComponent<SpawnPoint>().SetEnemyList(SplitEnemyList(nextWaveList, i, curLevelSpawnPoints.Count));
			curLevelSpawnPoints[i].GetComponent<SpawnPoint>().SendNextWave();
		}

		wavePanelScript.UpdatePanel();
	}

	// Calls each spawn point in given level to initialize
	public void InitializeSpawnPoints(int level)
	{
		curLevelSpawnPoints = boardManager.levelSpawnPoints[level];
		foreach (GameObject spawnPoint in boardManager.levelSpawnPoints[level])
		{
			spawnPoint.GetComponent<SpawnPoint>().Initialize(level);
		}
	}

	// Calls each spawnpoint in the current level to reinitialize
	public void RecalculatePaths()
	{
		curLevelSpawnPoints = boardManager.levelSpawnPoints[gameManager.currentLevel];
		foreach (GameObject spawnPoint in boardManager.levelSpawnPoints[gameManager.currentLevel])
		{
			spawnPoint.GetComponent<SpawnPoint>().Initialize(gameManager.currentLevel);
		}
	}

	// Calls AStar, Spawnpoints, and Enemies to change targets
	private void ChangeTarget(GameObject newTarget)
	{
		aStarScript.SetTarget(newTarget);

		foreach (GameObject spawnPoint in boardManager.levelSpawnPoints[gameManager.currentLevel])
		{
			spawnPoint.GetComponent<SpawnPoint>().SetTarget(newTarget);
		}

		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			enemy.GetComponent<Enemy>().SetTarget(newTarget);
		}
	}

	public bool EnemiesCanReachTarget()
	{
		foreach (GameObject spawnPoint in boardManager.levelSpawnPoints[gameManager.currentLevel])
		{
			if (!aStarScript.CalculateAStar(spawnPoint.transform.position) ||
			    !aStarScript.CalculateAStar(spawnPoint.transform.position, player.transform.position))
				return false;
		}
		return true;
	}

	// When players steps on the trigger tile for the next level
	public void TriggerNextLevel(int nextLevel)
	{
		// Start next level
		if (nextLevel > curLevel && nextLevel < boardManager.numLevels)
		{
			if (curLevel >= 0)
				ToggleBorderTiles(true, curLevel);
			gameManager.currentLevel = nextLevel;
			curLevel = nextLevel;
			player.GetComponent<TowerPlacement>().RecallAllTowers();
			ChangeTarget(boardManager.levelCages[curLevel]);
			InitializeSpawnPoints(nextLevel);
			levelComplete = false;
			waveNumber = -1;
			sendWave();
			inCooldown = false;
		}
	}

	public void TriggerCageDestroyed()
    {
        gameManager.DisplayMessage("Cage destroyed!");

        //Destroy(boardManager.levelCages[gameManager.currentLevel], 0);
		stopWave = inCooldownTimer;
		inCooldown = true;
		levelComplete = true;
		curLevelSpawnPoints = new List<GameObject>();
		// Debug.Log("Cage Destroyed!");
		aStarScript.CageOfftarget(curLevel);
		ChangeTarget(player);
	}

	public void TriggerCageUnlocked()
	{
        gameManager.DisplayMessage("Cage unlocked!");

	    //Destroy(boardManager.levelCages[gameManager.currentLevel], 0);
		stopWave = inCooldownTimer;
		inCooldown = true;
		levelComplete = true;
		curLevelSpawnPoints = new List<GameObject>();
		// Debug.Log("Cage Unlocked!");
		aStarScript.CageOfftarget(curLevel);
		ChangeTarget(player);
	}

	private List<GameObject> SplitEnemyList(List<GameObject> list, int index, int count)
	{
		float startIndex = Mathf.Ceil( index * ((float)list.Count / count) );
		float endIndex = (index+1) * ((float)list.Count / count);
		List<GameObject> newList = new List<GameObject>();

		for (float i = startIndex; i < endIndex && i < list.Count; i+=1)
		{
			newList.Add(list[(int)i]);
		}
		return newList;
	}
}
