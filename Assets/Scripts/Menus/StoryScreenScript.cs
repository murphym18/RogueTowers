using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TowerType = TestTowerScript.TowerType;
using UnityEngine.UI;

public class StoryScreenScript : MonoBehaviour {
	public GameObject hud;
	public GameObject dialogBox;
	
	public GameObject TyremFace;
	public GameObject AlreyFace;
	public GameObject ArckhanFace;
	public GameObject ClairaFace;
	public GameObject DawnFace;
	public GameObject LukuFace;
	public GameObject MaessFace;

	private GameObject levelTransitionNotice;
	private Dictionary<TowerType, string> towerToChar = new Dictionary<TowerType, string>();
	private Dictionary<string, GameObject> charToFace = new Dictionary<string, GameObject>();
	private TowerType currentTower;

	private string[] startLines;
	private string[] endLines;

	private int currentLine = 0;
	private string[] lines;

	void Start(){

		towerToChar.Add(TowerType.Bishop, "Clairy");
		towerToChar.Add(TowerType.King, "Arckhan");
		towerToChar.Add(TowerType.Knight, "Alry");
		towerToChar.Add(TowerType.Pawn, "Dawn");
		towerToChar.Add(TowerType.Queen, "Maess");
		towerToChar.Add(TowerType.Rook, "Luku");

		charToFace.Add ("Tyrem", TyremFace);
		charToFace.Add ("Alrey", AlreyFace);
		charToFace.Add ("rckhan", ArckhanFace);
		charToFace.Add ("Claira", ClairaFace);
		charToFace.Add ("Dawn", DawnFace);
		charToFace.Add ("Luku", LukuFace);
		charToFace.Add ("Maess", MaessFace);

		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.activeSelf) {
			Time.timeScale = 0;
			if (Input.anyKeyDown) {
				if (currentLine < lines.Length) {
					showDialog();
				}
				else {
					resume();
				}
			}
		}
	}

	public void showStart(int levelNumber, GameObject levelTransitionNotice) {
		if (levelNumber < 6 /*6 = the number of tower types*/) {
			currentTower = GameObject.Find ("GameManager").GetComponent<BoardManager> ().towers [levelNumber];
			this.levelTransitionNotice = levelTransitionNotice;
			gameObject.SetActive (true);
			loadText("start/" + towerToChar[currentTower]);
			showDialog();
		} else {
			resume();
		}
	}

	public void showEnd() {
		gameObject.SetActive (true);
		loadText ("end/" + towerToChar[currentTower]);
		showDialog();
	}

	public void resume() {
		hud.GetComponent<HUD>().show();
		gameObject.SetActive(false);
		levelTransitionNotice.SetActive(true);
		Destroy(levelTransitionNotice, 3.9f );
	}


	private void loadText(string path) {
		Debug.Log ("loading from " + path);
		string tmp = Resources.Load<TextAsset> (path).text;
		Debug.Log (tmp);
		lines = tmp.Split('\n');
		currentLine = 0;
	}

	private void showDialog() {
		string[] tmp = lines[currentLine++].Split ('\t');
		string character = tmp [0];
		string dialogText = tmp [1];
		dialogBox.GetComponent<Text> ().text = dialogText;
	}
}
