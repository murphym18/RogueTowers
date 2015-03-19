using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TT = TestTowerScript.TowerType;

public class TowerPlacement : MonoBehaviour {

    public static Dictionary<TT, int> TowerCount = new Dictionary<TT, int>();
    public static readonly Dictionary<TT, int> InitialTowers = new Dictionary<TT, int>(); 

	public static Dictionary<TT, GameObject> TowerGameObjects = new Dictionary<TT, GameObject>();
	public static Dictionary<TT, int> PlacedTowers = new Dictionary<TT, int>();
	public static Dictionary<TT, int> TowerKeys = new Dictionary<TT, int>();
	public static HashSet<TT> unlockedTowers = new HashSet<TT>();

    static TowerPlacement()
    {
        InitialTowers.Add(TT.Pawn, 4);
        InitialTowers.Add(TT.Knight, 2);
        InitialTowers.Add(TT.Bishop, 2);
        InitialTowers.Add(TT.Rook, 2);
        InitialTowers.Add(TT.King, 1);
        InitialTowers.Add(TT.Queen, 1);

        //Initialize(); // Also needs to be called when the game starts each time
    }
	
    public static void Initialize()
	{
        foreach (TT towerType in Enum.GetValues(typeof (TT)))
        {
            TowerCount[towerType] = -1;
            PlacedTowers[towerType] = 0;
        }
	}

	public GameObject PawnTower;
	public GameObject KnightTower;
	public GameObject BishopTower;
	public GameObject RookTower;
	public GameObject KingTower;
	public GameObject QueenTower;

	public GameObject rangeIndicatorObject;
	private GameObject rangeIndicatorInstance;
	
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
	private TT curType;
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

		TowerGameObjects.Clear();
		TowerGameObjects.Add(TT.Pawn, PawnTower);
		TowerGameObjects.Add(TT.Knight, KnightTower);
		TowerGameObjects.Add(TT.Bishop, BishopTower);
		TowerGameObjects.Add(TT.Rook, RookTower);
		TowerGameObjects.Add(TT.King, KingTower);
		TowerGameObjects.Add(TT.Queen, QueenTower);
	}

	void Start() {
		goodPlaceColor = new Color(0F, 1F, 0F, 0.25F);
		badPlaceColor = new Color (1F, 0F, 0F, 0.25F);

		curTowerKey = 1;
		TowerKeys.Clear();
		rangeIndicatorInstance = (GameObject) Instantiate(rangeIndicatorObject);
		rangeIndicatorInstance.SetActive(false);
	}

	public static void AddTowerType(TT towerType)
	{
		if (curTowerKey <= Enum.GetValues(typeof (TT)).Length)
		{
			string buttonName = "PlaceTower" + curTowerKey.ToString();
			towerInputKeys.Add(createTowerInput(buttonName, towerType));
			TowerKeys.Add(towerType, curTowerKey);
			curTowerKey++;
			TowerCount[towerType] = InitialTowers[towerType];

			hud.AddTowerButton(towerType);
		}
	}

	void Update () {

		foreach (TowerInput towerInput in towerInputKeys)
		{
			if (Input.GetButtonDown(towerInput.input)) {
				SelectTower(towerInput.type);
			}
		}

		/*
		if (Input.GetButtonDown("Cancel"))
			CancelSelectTower();
		*/

		if (Input.GetButtonDown("RecallTower")) {
			CancelSelectTower();
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
			rangeIndicatorInstance.transform.position = curTower.transform.position;
			if ((TowerCount[curType] - PlacedTowers[curType] > 0) &&
			    (useOldValid ? oldValid : isValidTowerPosition(x, y)))
			{
				curTower.GetComponent<SpriteRenderer>().color = goodPlaceColor;
				oldValid = true;
				if (Input.GetMouseButtonDown(0)) {
					PlaceTower(x, y);
					oldValid = false;
					rangeIndicatorInstance.SetActive(false);
				}
			}
			else {
				curTower.GetComponent<SpriteRenderer>().color = badPlaceColor;
				oldValid = false;
			}
		}
	}

	public void SelectTower(TT towerType)
	{
		if (curType == towerType && curTower!= null)
		{
			CancelSelectTower();
		}
		else
		{
			curType = towerType;
			curTower = instantiateTowerPointer(towerType);
			curTower.SetActive(true);
			colorBackup = curTower.GetComponent<SpriteRenderer>().color;
			recall = false;
			oldLocX = oldLocY = 0;

			float radiusScale = 2 * curTower.GetComponent<TestTowerScript>().attackRadius;
			rangeIndicatorInstance.transform.localScale = new Vector3(radiusScale, radiusScale, radiusScale);
			rangeIndicatorInstance.SetActive(true);
		}
	}

	public void CancelSelectTower()
	{
		if (curTower != null)
		{
			Destroy(curTower);
			curTower = null;
		}
		recall = false;
		rangeIndicatorInstance.SetActive(false);
	}
	
	private void PlaceTower(int x, int y)
	{
		curTower.SetActive(true);
		curTower.GetComponent<SpriteRenderer>().color = colorBackup;
		curTower.collider2D.enabled = true;
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
		TT type = ((ObjectXType)TowerMap[locationKey]).type;
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

	GameObject instantiateTowerPointer(TT towerType)
	{
		if (curTower)
		{
			Destroy(curTower);
		}

		GameObject towerPointer = Instantiate (TowerGameObjects[towerType], Vector3.zero, Quaternion.identity) as GameObject;
		towerPointer.SetActive(false);
		towerPointer.collider2D.enabled = false;
		towerPointer.GetComponent<TestTowerScript>().enabled = false;

		return towerPointer;
	}

	public bool IsTowerSelected()
	{
		return curTower != null;
	}

	private class TowerInput {
		public string input;
		public TT type;
	}

	private static TowerInput createTowerInput(string s, TT towerType) {
		TowerInput t = new TowerInput ();
		t.input = s;
		t.type = towerType;
		return t;
	}

	private class ObjectXType {
		public GameObject tower;
		public TT type;
		public ObjectXType(GameObject obj, TT t)
		{
			tower = obj;
			type = t;
		}
	}

	//private static TT Tower(string towerType)
	//{
	//	return (TT)Enum.Parse(typeof(TT), towerType);
	//}
}
