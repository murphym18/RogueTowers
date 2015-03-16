using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerPlacement : MonoBehaviour {

    public static Dictionary<TestTowerScript.TowerType, int> ExtraTowers = new Dictionary<TestTowerScript.TowerType, int>();
	public static Dictionary<TestTowerScript.TowerType, GameObject> TowerGameObjects = new Dictionary<TestTowerScript.TowerType, GameObject>();
	public static Dictionary<TestTowerScript.TowerType, int> PlacedTowers = new Dictionary<TestTowerScript.TowerType, int>();
	public static Dictionary<TestTowerScript.TowerType, int> TowerKeys = new Dictionary<TestTowerScript.TowerType, int>();

    static TowerPlacement()
    {
        Initialize(); // Also needs to be called when the game starts each time
    }
	
    private static void Initialize()
	{
		ExtraTowers.Add(Tower("Pawn"), 4);
		ExtraTowers.Add(Tower("Knight"), 2);
		ExtraTowers.Add(Tower("Bishop"), 2);
		ExtraTowers.Add(Tower("Rook"), 2);
		ExtraTowers.Add(Tower("King"), 1);
		ExtraTowers.Add(Tower("Queen"), 1);

		foreach (TestTowerScript.TowerType towerType in Enum.GetValues(typeof (TestTowerScript.TowerType))) PlacedTowers.Add(towerType, 0);
	}

	public GameObject PawnTower;
	public GameObject KnightTower;
	public GameObject BishopTower;
	public GameObject RookTower;
	public GameObject KingTower;
	public GameObject QueenTower;
	
	private GameManager gameManager;
	private BoardManager boardManager;
	private AStar aStarScript;
	private static HUD hud;
	private int levelWidth;
	private GameObject curTower = null;
	private static List<TowerInput> towerInputKeys = new List<TowerInput>();
	private Hashtable TowerMap = new Hashtable();
	private bool recall = false;
	private int oldLocX = 0, oldLocY = 0;
	private bool oldValid = false;
	private TestTowerScript.TowerType curType;
	private static int curTowerKey = 1;

	private Color goodPlaceColor;
	private Color badPlaceColor;
	private Color colorBackup;
	
	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		boardManager = gameManager.GetComponentInParent<BoardManager>();
		aStarScript = gameManager.GetComponentInParent<AStar>();
		hud = GameObject.Find("HUD").GetComponent<HUD>();
		levelWidth = boardManager.MapWidth / boardManager.numLevels;

		TowerGameObjects.Add(Tower("Pawn"), PawnTower);
		TowerGameObjects.Add(Tower("Knight"), KnightTower);
		TowerGameObjects.Add(Tower("Bishop"), BishopTower);
		TowerGameObjects.Add(Tower("Rook"), RookTower);
		TowerGameObjects.Add(Tower("King"), KingTower);
		TowerGameObjects.Add(Tower("Queen"), QueenTower);
	}

	void Start() {
		goodPlaceColor = new Color(0F, 1F, 0F, 0.25F);
		badPlaceColor = new Color (1F, 0F, 0F, 0.25F);
	}

	public static void AddTowerType(TestTowerScript.TowerType towerType)
	{
		string buttonName = "PlaceTower" + curTowerKey.ToString();
		towerInputKeys.Add(createTowerInput(buttonName, towerType));
		TowerKeys.Add(towerType, curTowerKey);
		curTowerKey++;

		hud.AddTowerButton(towerType);
	}

	void Update () {

		foreach (TowerInput towerInput in towerInputKeys)
		{
			if (Input.GetButtonDown(towerInput.input)) {
				SelectTower(towerInput.type);
			}
		}

		if (Input.GetButtonDown("Cancel")) {
			if (curTower != null)
			{
				Destroy(curTower);
				curTower = null;
			}
			recall = false;
		}

		if (Input.GetButtonDown("RecallTower")) {
			if (curTower != null)
			{
				Destroy(curTower);
				curTower = null;
			}
			recall = true;
		}

		if (recall)
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 worldLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				worldLocation.x = (int) Mathf.Round(worldLocation.x);
				worldLocation.y = (int) Mathf.Round(worldLocation.y);
				worldLocation.z = 0;

				string locationKey = ((int)worldLocation.x).ToString() + '_' + ((int)worldLocation.y).ToString();
				if (TowerMap.ContainsKey(locationKey))
				{
					RecallTower(locationKey);
					recall = false;
				}
			}
		}
		else if (curTower != null) {
			Vector3 worldLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int x = (int) Mathf.Round(worldLocation.x);
			int y = (int) Mathf.Round(worldLocation.y);
			bool useOldValid = false;

			if (oldLocX == x && oldLocY == y)
			{
				useOldValid = true;
			}
			else {	
				oldLocX = x;
				oldLocY = y;
			}

			curTower.transform.position = new Vector2(x, y);
			if ((ExtraTowers[curType] - PlacedTowers[curType] > 0) &&
			    (useOldValid ? oldValid : isValidTowerPosition(x, y)))
			{
				curTower.GetComponent<SpriteRenderer>().color = goodPlaceColor;
				oldValid = true;
				if (Input.GetMouseButtonDown(0)) {
					PlaceTower(x, y);
					oldValid = false;
				}
			}
			else {
				curTower.GetComponent<SpriteRenderer>().color = badPlaceColor;
				oldValid = false;
			}
		}
	}

	public void SelectTower(TestTowerScript.TowerType towerType)
	{
		curType = towerType;
		curTower = instantiateTowerPointer(towerType);
		curTower.SetActive(true);
		colorBackup = curTower.GetComponent<SpriteRenderer>().color;
		recall = false;
		oldLocX = oldLocY = 0;
	}

	private void PlaceTower(int x, int y)
	{
		curTower.SetActive(true);
		curTower.GetComponent<SpriteRenderer>().color = colorBackup;
		curTower.GetComponent<BoxCollider2D>().enabled = true;
		curTower.GetComponent<TestTowerScript>().enabled = true;

		TowerMap.Add(x.ToString() + '_' + y.ToString(), new ObjectXType(curTower, curType));
		aStarScript.PlacedObstacleAt(x, y);
		PlacedTowers[curType]++;
		hud.PrepareChildren();
		curTower = null;
	}

	private void RecallTower(string locationKey)
	{
		GameObject tower = ((ObjectXType)TowerMap[locationKey]).tower;
		TestTowerScript.TowerType type = ((ObjectXType)TowerMap[locationKey]).type;
		boardManager[(int)tower.transform.position.x, (int)tower.transform.position.y] = false;
		Destroy(tower);
		TowerMap.Remove(locationKey);
		PlacedTowers[type]--;
		hud.PrepareChildren();

		oldLocX = oldLocY = 0;
	}

	public void RecallAllTowers()
	{
		IEnumerator towerEnumerator = TowerMap.Values.GetEnumerator();
		while (towerEnumerator.MoveNext())
		{
			GameObject tower = ((ObjectXType)towerEnumerator.Current).tower;
			boardManager[(int)tower.transform.position.x, (int)tower.transform.position.y] = false;
			Destroy(tower);
			PlacedTowers[((ObjectXType)towerEnumerator.Current).type]--;
		}
		TowerMap.Clear();
		hud.PrepareChildren();
	}

	// TODO
	private bool CanRemoveTowers()
	{
		return true;
	}

	// Check if a tower can legally be placed at given x,y position.
	// can optimize with a hashtable
	bool isValidTowerPosition(int x, int y)
	{
		if (!boardManager[x, y] &&
		    !(x == (int)Mathf.Round(transform.position.x) &&
		  	  y == (int)Mathf.Round(transform.position.y)))
		{
			int levelStart_x = levelWidth * gameManager.currentLevel;
			int levelEnd_x = levelStart_x + levelWidth;
			if (x > levelStart_x && x < levelEnd_x)
			{
				if (!isPotentialBlock(x,y))
					return true;
				curTower.GetComponent<BoxCollider2D>().enabled = true;
				bool reached = aStarScript.TestObstacleAt(x, y);
				curTower.GetComponent<BoxCollider2D>().enabled = false;
				aStarScript.ReformAdjNodeMesh();
				return reached;
			}
		}
		return false;
	}

	private bool isPotentialBlock(int x, int y)
	{
		int[] checks = new int[16]{x-1,y-1, x-1,y, x-1,y+1, x,y+1, x+1,y+1, x+1,y, x+1,y-1, x,y-1};
		int changes = 0;

		for (int cur_x=0, cur_y=1; cur_x < checks.Length-2; cur_x+=2,cur_y+=2)
		{
			if (boardManager[checks[cur_x],checks[cur_y]] != boardManager[checks[cur_x+2],checks[cur_y+2]])
			{
				if (++changes > 2)
					return true;
			}
		}
		return false;
	}

	GameObject instantiateTowerPointer(TestTowerScript.TowerType towerType)
	{
		if (curTower)
		{
			Destroy(curTower);
		}

		GameObject towerPointer = Instantiate (TowerGameObjects[towerType], Vector3.zero, Quaternion.identity) as GameObject;
		towerPointer.SetActive(false);
		towerPointer.GetComponent<BoxCollider2D>().enabled = false;
		towerPointer.GetComponent<TestTowerScript>().enabled = false;

		return towerPointer;
	}

	private class TowerInput {
		public string input;
		public TestTowerScript.TowerType type;
	}

	private static TowerInput createTowerInput(string s, TestTowerScript.TowerType towerType) {
		TowerInput t = new TowerInput ();
		t.input = s;
		t.type = towerType;
		return t;
	}

	private class ObjectXType {
		public GameObject tower;
		public TestTowerScript.TowerType type;
		public ObjectXType(GameObject obj, TestTowerScript.TowerType t)
		{
			tower = obj;
			type = t;
		}
	}

	private static TestTowerScript.TowerType Tower(string towerType)
	{
		return (TestTowerScript.TowerType)Enum.Parse(typeof(TestTowerScript.TowerType), towerType);
	}
}
