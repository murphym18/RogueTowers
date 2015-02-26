using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour {

	public GameObject[] enemySpawns;
	public AStar aStarScript;

	public int spawnInterval = 3;
	private List<Vector3> pathPoints;
	public int nextWaveLevel = 1;

	// Use this for initialization
	void Start () {
		// Get A* path to Cage

		aStarScript = GetComponent<AStar>();
		pathPoints = aStarScript.GetPoints();

		// Spawn enemies
		StartCoroutine(SpawnAtInterval(spawnInterval, nextWaveLevel++));
	}
	
	// Update is called once per frame
	void Update () {
	
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
				SpawnEnemy(enemyType);
				yield return new WaitForSeconds(seconds);
			}
		}
	}

	void SpawnEnemy(GameObject enemyType)
	{
		GameObject newEnemy = (GameObject)Instantiate (enemyType, transform.position, Quaternion.identity);
		newEnemy.SendMessage("SetSpawnPoint", this);
		newEnemy.SendMessage("MyActivate");
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
