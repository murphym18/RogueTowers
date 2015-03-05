using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class EnemyLayoutScript : MonoBehaviour {

	public GameObject spawnPoint;
	public int spawnPointCount = 1;
	public int enemyPocketCount = 3;
	public int enemyPocketSize = 5;

	private GameObject gameScript;
	private BoardManager boardScript;
	private int mapWidth, mapHeight;

	// main function to set up enemies and spawnpoints in existing map
	public void SetupEnemyLayout()
	{
		gameScript = GameObject.Find("GameManager");
		boardScript = gameScript.GetComponent<BoardManager>();
		mapWidth = boardScript.MapWidth;
		mapHeight = boardScript.MapHeight;

		SetupEnemySpawnPoints(spawnPointCount);
		SetupEnemyPockets(enemyPocketCount, enemyPocketSize);
	}

	// TODO: don't spawn multiple on same tile
	void SetupEnemySpawnPoints(int spawnPointCount)
	{
		while (spawnPointCount > 0)
		{
			int x = Random.Range (1, mapWidth);
			int y = Random.Range (1, mapHeight);
			if (!boardScript[x, y])
			{
				GameObject instance = Instantiate(spawnPoint, new Vector3(x, y), Quaternion.identity) as GameObject;
				instance.transform.SetParent(boardScript.boardHolder);

				// initialize the spawn point
				instance.GetComponent<SpawnPoint>().Initialize();

				spawnPointCount--;
			}
		}
	}

	// TODO: this
	void SetupEnemyPockets(int enemyPocketCount, int enemyPocketSize)
	{

	}
}
