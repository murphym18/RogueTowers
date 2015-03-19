using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

using Helpers;
using Random = UnityEngine.Random;
using TowerType = TestTowerScript.TowerType;
public class BoardManager : MonoBehaviour
{

	public GameObject wallPrefab;
	public int numWallSpiresPerTheme = 4;
	public GameObject floorPrefab;
	public int numFloorSpiresPerTheme = 1;

    public GameObject borderTile;
    public GameObject levelTriggerTile;
    public GameObject chestTile;
    public GameObject nextLevelObject;
    private GameObject[] nextLevelNotices;
    private MapBuilder map;
    public int numLevels, levelWidth, levelHeight;
    public int minChestsPerLevel, maxChestsPerLevel;
    public bool useFloorEverywhere = false;

	public GameObject[] levelCages;
	public List<GameObject>[] levelSpawnPoints;
	public List<GameObject>[] levelBorderTiles;

	public Transform boardHolder;

	public TowerType[] towers;
	Dictionary<TowerType, TileSet> tileSets = new Dictionary<TowerType, TileSet>();

    bool chestPlacement(int x, int y)
    {
        return map[x - 1, y] || map[x + 1, y] || map[x, y - 1] || map[x, y + 1];
    }

	void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
    }

	private void setupTileSets() {
		towers = TowerPlacement.InitialTowers.Keys.toShuffledArray();
		foreach (var tower in towers) {
			Sprite[] floorTiles = new Sprite[numFloorSpiresPerTheme];
			Sprite[] wallTiles = new Sprite[numWallSpiresPerTheme];
			
			for (int i = 0; i < numFloorSpiresPerTheme; ++i) {
				floorTiles[i] = loadFromImage("Floors/" + tower.ToString() + i);
			}
			for (int i = 0; i < numWallSpiresPerTheme; ++i) {
				wallTiles[i] = loadFromImage("Walls/" + tower.ToString() + i);
			}
			tileSets.Add(tower, new TileSet(floorPrefab, floorTiles, wallPrefab, wallTiles));
		}
	}

	Rect imgRect = new Rect(0F, 0F, 48F, 48F);
	Vector2 imgPiv = new Vector2(0.5F, 0.5F);
	float imgPixelsToUnits = 48F;

	private Sprite loadFromImage(string img) {
		TextAsset imgData = Resources.Load<TextAsset>(img);
		Texture2D tex = new Texture2D(1,1);
		tex.LoadImage(imgData.bytes);
		return Sprite.Create (tex, imgRect, imgPiv, imgPixelsToUnits);
	}



    private void GenerateMap()
    {
        map = new MapBuilder(numLevels, levelWidth, levelHeight);
    }

    private void LayoutMap()
    {
		levelBorderTiles = new List<GameObject>[numLevels];
        nextLevelNotices = new GameObject[numLevels];
        for (int level = 0; level < numLevels; level++)
        {
			TowerType tower = towers[level % towers.Length];
            levelBorderTiles[level] = new List<GameObject>();
            nextLevelNotices[level] = Instantiate(nextLevelObject, new Vector2(level*levelWidth, 0.5f*levelHeight + 2),
                Quaternion.identity) as GameObject;
            nextLevelNotices[level].SetActive(false);
			for (int y = 0; y < levelHeight; y++)
				for (int x = level*levelWidth; x < (level + 1)*levelWidth; x++)
				{
					GameObject instance = null;
					if (useFloorEverywhere || !map[x, y])
					{
						instance = tileSets[tower].createFloorTile(x, y);
						instance.transform.SetParent(boardHolder);
					}
					if (map[x, y])
					{
						instance = tileSets[tower].createWallTile(x, y);
						instance.transform.SetParent(boardHolder);
					}
					else if (x%levelWidth == levelWidth - 1)
					{
						instance = Instantiate(borderTile, new Vector3(x, y), Quaternion.identity) as GameObject;
						instance.transform.SetParent(boardHolder);
						map[x, y] = true;
						levelBorderTiles[x / levelWidth].Add(instance);
					}
					if (x%levelWidth == 1 && !map[x-1,y])
					{
						instance = Instantiate(levelTriggerTile, new Vector3(x, y), Quaternion.identity) as GameObject;
						instance.GetComponent<LevelTrigger>().levelTransitionNotice = nextLevelNotices[(x-1)/levelWidth];
						instance.transform.SetParent(boardHolder);
						instance.GetComponent<LevelTrigger>().Level = (int) ((x - 1)/levelWidth);
					}
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
		setupTileSets();
        BoardSetup();
        GenerateMap();
        LayoutMap();
        CreateForeground();

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



	public class TileSet {
		public GameObject floorPrefab;
		public Sprite[] floorTiles;
		public GameObject wallPrefab;
		public Sprite[] wallTiles;
	
		public TileSet(GameObject floorPrefab, Sprite[] floorTiles, GameObject wallPrefab, Sprite[] wallTiles) {
			this.floorPrefab = floorPrefab;
			this.floorTiles = floorTiles;
			this.wallPrefab = wallPrefab;
			this.wallTiles = wallTiles;
		}

		public GameObject createFloorTile(float x, float y) {
			return createTile(floorPrefab, floorTiles, x, y);
		}

		public GameObject createWallTile(float x, float y) {
			return createTile(wallPrefab, wallTiles, x, y);
		}

		private GameObject createTile(GameObject prefab, Sprite[] imgs, float x, float y) {
			GameObject tile = Instantiate(prefab, new Vector3(x, y), Quaternion.identity) as GameObject;
			SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
			renderer.sprite = imgs.RandomChoice();
			renderer.sortingLayerName = "Background";
			return tile;
		}
	}
}
