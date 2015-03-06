using UnityEngine;
using System.Collections;

public class TowerPlacement : MonoBehaviour {

	public GameObject PawnTower;
	public GameObject KnightTower;
	public GameObject BishopTower;
	public GameObject RookTower;
	public GameObject KingTower;
	public GameObject QueenTower;

	private GameObject gameManagerObject;
	private GameManager gameManager;
	private BoardManager boardManager;
	private int mapWidth, mapHeight, levelWidth, numLevels;
	private GameObject curTower = null;
	private TowerInput[] TowerInputKeys;

	private Color goodPlaceColor;
	private Color badPlaceColor;
	private Color colorBackup;

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

	void Awake()
	{
		gameManagerObject = GameObject.Find("GameManager");
		gameManager = gameManagerObject.GetComponent<GameManager>();
		boardManager = gameManagerObject.GetComponent<BoardManager>();
		mapWidth = boardManager.MapWidth;
		mapHeight = boardManager.MapHeight;
		numLevels = boardManager.numLevels;
		levelWidth = mapWidth / numLevels;
	}

	void Update () {


		for (int i = 0; i < TowerInputKeys.Length && curTower == null; ++i) {
			if (Input.GetButtonDown(TowerInputKeys[i].input)) {
				curTower = instantiateTowerPointer(TowerInputKeys[i].Tower);
				curTower.SetActive(true);
				colorBackup = curTower.GetComponent<SpriteRenderer>().color;
			}
		}

		if (curTower != null && Input.GetButtonDown("Cancel")) {
			Destroy(curTower);
			curTower = null;
		}

		if (curTower != null) {
			Vector3 worldLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			worldLocation.x = (int) Mathf.Round(worldLocation.x);
			worldLocation.y = (int) Mathf.Round(worldLocation.y);
			worldLocation.z = 0;

			if (isValidTowerPosition(worldLocation))
			{
				curTower.transform.position = new Vector2(worldLocation.x, worldLocation.y);
				curTower.GetComponent<SpriteRenderer>().color = goodPlaceColor;
				if (Input.GetMouseButtonDown(0)) {
					curTower.SetActive(true);
					curTower.GetComponent<SpriteRenderer>().color = colorBackup;
					curTower.GetComponent<BoxCollider2D>().enabled = true;
					curTower.GetComponent<TestTowerScript>().enabled = true;
					curTower = null;
				}
			}
			else {
				curTower.transform.position = new Vector2(worldLocation.x, worldLocation.y);
				curTower.GetComponent<SpriteRenderer>().color = badPlaceColor;
				//curTower.SetActive(false);
			}



		}
	}

	// Check if a tower can legally be placed at given x,y position.
	bool isValidTowerPosition(Vector3 locationVector)
	{
		int levelStart_x = levelWidth * gameManager.currentLevel;
		int levelEnd_x = levelStart_x + levelWidth;
		if (locationVector.x > levelStart_x && locationVector.x < levelEnd_x)
			return !boardManager[(int)locationVector.x,(int)locationVector.y];
		else
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
