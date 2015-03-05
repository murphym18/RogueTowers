using UnityEngine;
using System.Collections;

public class TowerPlacement : MonoBehaviour {

	public GameObject PawnTower;

	private GameObject gameManager;
	private BoardManager boardManager;
	private GameObject curTower = null;

	void Awake()
	{
		gameManager = GameObject.Find("GameManager");
		boardManager = gameManager.GetComponent<BoardManager>();
	}

	void Update () {
	
		if (Input.GetButtonDown("PlaceTowerPawn")) {
			curTower = instantiateTowerPointer(PawnTower);
		}
		else if (Input.GetButtonDown("Cancel")) {
			Destroy(curTower);
			curTower = null;
		}

		if (curTower != null)
		{
			Vector3 worldLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			worldLocation.x = (int) Mathf.Round(worldLocation.x);
			worldLocation.y = (int) Mathf.Round(worldLocation.y);
			worldLocation.z = 0;

			if (isValidTowerPosition(worldLocation))
			{
				curTower.transform.position = new Vector2(worldLocation.x, worldLocation.y);
				curTower.SetActive(true);
			}
			else
			{
				curTower.SetActive(false);
			}

			if (Input.GetMouseButtonDown(0))
			{
				curTower.SetActive(true);
				curTower.GetComponent<BoxCollider2D>().enabled = true;
				curTower.GetComponent<TestTowerScript>().enabled = true;

				curTower = null;
			}
		}

	}

	// Check if a tower can legally be placed at given x,y position.
	bool isValidTowerPosition(Vector3 locationVector)
	{
		return !boardManager[(int)locationVector.x,(int)locationVector.y];
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
}
