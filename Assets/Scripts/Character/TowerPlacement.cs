using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerPlacement : MonoBehaviour {

    public static Dictionary<TestTowerScript.TowerType, int> ExtraTowers = new Dictionary<TestTowerScript.TowerType, int>();

    static TowerPlacement()
    {
        Initialize(); // Also needs to be called when the game starts each time
    }
	
    private static void Initialize()
	{
		ExtraTowers.Add((TestTowerScript.TowerType)Enum.Parse(typeof(TestTowerScript.TowerType), "Pawn"), 4);
		ExtraTowers.Add((TestTowerScript.TowerType)Enum.Parse(typeof(TestTowerScript.TowerType), "Knight"), 2);
		ExtraTowers.Add((TestTowerScript.TowerType)Enum.Parse(typeof(TestTowerScript.TowerType), "Bishop"), -1);
		ExtraTowers.Add((TestTowerScript.TowerType)Enum.Parse(typeof(TestTowerScript.TowerType), "Rook"), -1);
		ExtraTowers.Add((TestTowerScript.TowerType)Enum.Parse(typeof(TestTowerScript.TowerType), "King"), 1);
		ExtraTowers.Add((TestTowerScript.TowerType)Enum.Parse(typeof(TestTowerScript.TowerType), "Queen"), -1);
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
	private int levelWidth;
	private GameObject curTower = null;
	private TowerInput[] TowerInputKeys;
	private Hashtable TowerMap = new Hashtable();
	private bool recall = false;
	private int oldLocX = 0, oldLocY = 0;
	private bool oldValid = false;

	private Color goodPlaceColor;
	private Color badPlaceColor;
	private Color colorBackup;
	
	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		boardManager = gameManager.GetComponentInParent<BoardManager>();
		aStarScript = gameManager.GetComponentInParent<AStar>();
		levelWidth = boardManager.MapWidth / boardManager.numLevels;
	}

	void Start() {
		goodPlaceColor = new Color(0F, 1F, 0F, 0.25F);
		badPlaceColor = new Color (1F, 0F, 0F, 0.25F);
	 	TowerInputKeys = new TowerInput[] {
			createTowerInput("PlaceTowerPawn", PawnTower),
			createTowerInput("PlaceTowerKnight", KnightTower),
			createTowerInput("PlaceTowerBishop", BishopTower),
			createTowerInput("PlaceTowerRook", RookTower),
			createTowerInput("PlaceTowerKing", KingTower),
			createTowerInput("PlaceTowerQueen", QueenTower)
		};
	}

	void Update () {

		for (int i = 0; i < TowerInputKeys.Length && curTower == null; ++i) {
			if (Input.GetButtonDown(TowerInputKeys[i].input)) {
				curTower = instantiateTowerPointer(TowerInputKeys[i].Tower);
				curTower.SetActive(true);
				colorBackup = curTower.GetComponent<SpriteRenderer>().color;
				recall = false;
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
			if (useOldValid ? oldValid : isValidTowerPosition(x, y))
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

	private void PlaceTower(int x, int y)
	{
		curTower.SetActive(true);
		curTower.GetComponent<SpriteRenderer>().color = colorBackup;
		curTower.GetComponent<BoxCollider2D>().enabled = true;
		curTower.GetComponent<TestTowerScript>().enabled = true;

		TowerMap.Add(x.ToString() + '_' + y.ToString(), curTower);
		aStarScript.PlacedObstacleAt(x, y);
		curTower = null;
	}

	private void RecallTower(string locationKey)
	{
		GameObject tower = TowerMap[locationKey] as GameObject;
		boardManager[(int)tower.transform.position.x, (int)tower.transform.position.y] = false;
		Destroy(tower);
		TowerMap.Remove(locationKey);
	}

	public void RecallAllTowers()
	{
		IEnumerator towerEnumerator = TowerMap.Values.GetEnumerator();
		while (towerEnumerator.MoveNext())
		{
			GameObject tower = towerEnumerator.Current as GameObject;
			boardManager[(int)tower.transform.position.x, (int)tower.transform.position.y] = false;
			Destroy(tower);
		}
		TowerMap.Clear();
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

	GameObject instantiateTowerPointer(GameObject towerType)
	{
		if (curTower)
		{
			Destroy(curTower);
		}

		GameObject towerPointer = Instantiate (towerType, Vector3.zero, Quaternion.identity) as GameObject;
		towerPointer.SetActive(false);
		towerPointer.GetComponent<BoxCollider2D>().enabled = false;
		towerPointer.GetComponent<TestTowerScript>().enabled = false;

		return towerPointer;
	}

	private class TowerInput {
		public string input;
		public GameObject Tower;
	}

	private TowerInput createTowerInput(string s, GameObject g) {
		TowerInput t = new TowerInput ();
		t.input = s;
		t.Tower = g;
		return t;
	}
}
