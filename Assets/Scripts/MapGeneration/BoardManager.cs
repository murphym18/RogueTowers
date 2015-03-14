using UnityEngine;
using System.Collections.Generic;
using System;
using Helpers;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
	public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject borderTile;
    public GameObject levelTriggerTile;
    public GameObject chestTile;
    private MapBuilder map;
    public int numLevels, levelWidth, levelHeight;
    public int minChestsPerLevel, maxChestsPerLevel;
    public bool useFloorEverywhere = false;

	public GameObject[] levelCages;
	public List<GameObject>[] levelSpawnPoints;
	public List<GameObject>[] levelBorderTiles;

	public Transform boardHolder;

    bool chestPlacement(int x, int y)
    {
        return map[x - 1, y] || map[x + 1, y] || map[x, y - 1] || map[x, y + 1];
    }

	void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
    }

    private void GenerateMap()
    {
        map = new MapBuilder(numLevels, levelWidth, levelHeight);
    }

    private void LayoutMap()
    {
		levelBorderTiles = new List<GameObject>[numLevels];
		for (int level = 0; level < numLevels; level++)
			levelBorderTiles[level] = new List<GameObject>();

		for (int y = 0; y < levelHeight; y++)
            for (int x = 0; x < numLevels * levelWidth; x++)
            {
                UnityEngine.Object instance = null;
                if (useFloorEverywhere || !map[x, y])
                {
                    instance = Instantiate(floorTiles.RandomChoice(), new Vector3(x, y), Quaternion.identity);
                    ((GameObject) instance).transform.SetParent(boardHolder);
                }
                if (map[x, y])
                {
                    instance = Instantiate(wallTiles.RandomChoice(), new Vector3(x, y), Quaternion.identity);
                    ((GameObject)instance).transform.SetParent(boardHolder);
                }
                else if (x%levelWidth == levelWidth - 1)
                {
                    instance = Instantiate(borderTile, new Vector3(x, y), Quaternion.identity);
                    ((GameObject)instance).transform.SetParent(boardHolder);
                    map[x, y] = true;
					levelBorderTiles[x / levelWidth].Add((GameObject)instance);
                }
                if (x%levelWidth == 1 && !map[x-1,y])
                {
                    instance = Instantiate(levelTriggerTile, new Vector3(x, y), Quaternion.identity);
                    ((GameObject)instance).transform.SetParent(boardHolder);
                    ((GameObject) instance).GetComponent<LevelTrigger>().Level = (int) ((x - 1)/levelWidth);
                }
            }
    }

    private void ScatterObjects(int Count, GameObject ToGenerate, int XMin = 0, int XMax = -1, Func<int, int, bool> Condition = null)
    {
        XMax = XMax == -1 ? numLevels*levelWidth : XMax;
        Condition = Condition ?? new Func<int, int, bool>((x, y) => true);
        while (Count > 0)
        {
            int x = Random.Range(XMin, XMax);
            int y = Random.Range(0, levelHeight);
            if (!map[x, y] && Condition(x, y))
            {
                map[x, y] = true;
                Instantiate(ToGenerate, new Vector3(x, y), Quaternion.identity);
                Count--;
            }
        }
    }

    private void CreateForeground()
    {
        for (int lvl = 0; lvl < numLevels; lvl++)
        {
            int left = lvl*levelWidth;
            int right = left + levelWidth;
            ScatterObjects(Random.Range(minChestsPerLevel, maxChestsPerLevel), chestTile, left, right, chestPlacement);
        }
    }

    public void SetupScene()
    {
        BoardSetup();
        GenerateMap();
        LayoutMap();
        Debug.Log(map);
        CreateForeground();
        Debug.Log(map);

		// Setup enemy spawnpoints and also cages
		GetComponent<EnemyLayoutScript>().SetupEnemyLayout();

		// tell AStar and WaveManger to start
		GetComponent<AStar>().enabled = true;
		GetComponent<WaveManagerScript>().enabled = true;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Gets or sets the blockability of the specified tile
    /// </summary>
    /// <param name="x">The grid's x coordinate to check</param>
    /// <param name="y">The grid's y coordinate to check</param>
    /// <returns>True if the specified grid location is passable, false otherwise</returns>
    public bool this[int x, int y]
    {
        get { return map[x, y]; }
        set { map[x, y] = value; }
    }

    /// <summary>
    /// Gets the width of the entire map (in tiles)
    /// </summary>
    public int MapWidth
    {
        get { return numLevels * levelWidth; }
    }

    /// <summary>
    /// Gets the height of the entire map (in tiles)
    /// </summary>
    public int MapHeight
    {
        get { return levelHeight; }
    }
}
