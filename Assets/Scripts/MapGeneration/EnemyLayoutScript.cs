using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class EnemyLayoutScript : MonoBehaviour {

	public GameObject SpawnPointObject;
	public GameObject CageObject;
	public int spawnPointCount = 1;
	public int enemyPocketCount = 3;
	public int enemyPocketSize = 5;

	private GameObject gameScript;
	private BoardManager boardScript;
	private int mapWidth, mapHeight, levelWidth;
	private int numLevels;
	private int cageOffset_x;
	private int cageOffset_y;

	// main function to set up enemies and spawnpoints in existing map
	public void SetupEnemyLayout()
	{
		gameScript = GameObject.Find("GameManager");
		boardScript = gameScript.GetComponent<BoardManager>();
		mapWidth = boardScript.MapWidth;
		mapHeight = boardScript.MapHeight;
		numLevels = boardScript.numLevels;
		levelWidth = mapWidth / numLevels;

		cageOffset_x = 4;
		cageOffset_y = (int)(mapHeight/2);

		boardScript.LevelCages = new GameObject[numLevels];

		SetupCages(numLevels); // TODO: will be numLevels
		SetupEnemySpawnPoints(spawnPointCount);
		SetupEnemyPockets(enemyPocketCount, enemyPocketSize);
		Debug.Log(boardScript.LevelCages[0]);
	}

	void SetupCages(int levels)
	{
		for (int curLevel = 0; curLevel < levels; curLevel++)
		{
			int levelOffset = levelWidth * curLevel;
			GameObject instance = Instantiate(CageObject, new Vector3(levelOffset + cageOffset_x, cageOffset_y), Quaternion.identity) as GameObject;
			boardScript.LevelCages[curLevel] = instance;
		}
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
				GameObject instance = Instantiate(SpawnPointObject, new Vector3(x, y), Quaternion.identity) as GameObject;
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
