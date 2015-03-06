using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour {

	public GameObject[] enemySpawns;
	public int spawnInterval = 3;
	public int nextWaveLevel = 1;

	private AStar aStarScript;
	private List<Vector3> pathPoints;
	private GameObject gameManager;
	private BoardManager boardManager;

	void Awake()
	{
		gameManager = GameObject.Find("GameManager");
		boardManager = gameManager.GetComponent<BoardManager>();
		aStarScript = GetComponent<AStar>();
	}

	public void Initialize(int level)
	{
		aStarScript.SetTarget(boardManager.levelCages[level]);
		aStarScript.Initialize();
		pathPoints = aStarScript.GetPoints();
	}

	IEnumerator SpawnAtInterval(int seconds, int waveLevel)
	{
		int maxEnemies = enemySpawns.Length * waveLevel;
		int enemyCount = 0;

		while (enemyCount < maxEnemies)
		{
			foreach (GameObject enemyType in enemySpawns)
			{
				if (enemyCount++ > maxEnemies)
					break;
				
				if (enabled)
					SpawnEnemy(enemyType);

				yield return new WaitForSeconds(seconds);
			}
		}
	}

	void SpawnEnemy(GameObject enemyType)
	{
		GameObject newEnemy = (GameObject)Instantiate (enemyType, transform.position, Quaternion.identity);
		newEnemy.GetComponent<Enemy>().Initialize(this);
		newEnemy.GetComponent<Enemy>().enabled = true;
	}

	public List<Vector3> GetPathPoints()
	{
		return pathPoints;
	}

	public void sendNextWave()
	{
		StartCoroutine(SpawnAtInterval(spawnInterval, nextWaveLevel++));
	}

}
