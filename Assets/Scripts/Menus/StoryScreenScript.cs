﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TowerType = TestTowerScript.TowerType;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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
	public GameObject EnemyFace;
	public GameObject LowerThird;

	private GameObject levelTransitionNotice;
	private Dictionary<TowerType, string> towerToChar = new Dictionary<TowerType, string>();
	private Dictionary<string, GameObject> charToFace = new Dictionary<string, GameObject>();
	private TowerType currentTower;

	private string[] startLines;
	private string[] endLines;

	private int currentLine = 0;
	private string[] lines;

	void Start(){

        towerToChar.Add(TowerType.Bishop, "Claira".ToLower());
        towerToChar.Add(TowerType.King, "Arckhan".ToLower());
        towerToChar.Add(TowerType.Knight, "Alrey".ToLower());
        towerToChar.Add(TowerType.Pawn, "Dawn".ToLower());
        towerToChar.Add(TowerType.Queen, "Maess".ToLower());
        towerToChar.Add(TowerType.Rook, "Luku".ToLower());

	    charToFace.Add("Tyrem".ToLower(), TyremFace);
        charToFace.Add("Alrey".ToLower(), AlreyFace);
        charToFace.Add("Arckhan".ToLower(), ArckhanFace);
        charToFace.Add("Claira".ToLower(), ClairaFace);
        charToFace.Add("Dawn".ToLower(), DawnFace);
        charToFace.Add("Luku".ToLower(), LukuFace);
        charToFace.Add("Maess".ToLower(), MaessFace);
        charToFace.Add("Enemy".ToLower(), EnemyFace);

		//gameObject.SetActive(false);
		showHelp ();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.activeSelf) {
			Time.timeScale = 0;
			if (Input.GetMouseButtonDown(0)) {
				if (currentLine < lines.Length) {
					showDialog();
				}
				else {
					resume();
				}
			}
		}
	}

	public void showHelp() {
		gameObject.SetActive (true);
		loadText ("start/help");
		showDialog();
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
	    if (levelTransitionNotice != null)
	    {
	        levelTransitionNotice.SetActive(true);
	        Destroy(levelTransitionNotice, 3.9f);
	    }
	}


	private void loadText(string path) {
		// Debug.Log ("loading from " + path);
		string tmp = Resources.Load<TextAsset> (path).text;
		// Debug.Log (tmp);
		lines = tmp.Split('\n');
		currentLine = 0;
	}

	private void clearDialog() {
		if (LowerThird.transform.childCount > 1) {
			Destroy (LowerThird.transform.GetChild (1).gameObject);
		}
		dialogBox.GetComponent<Text>().text = "";
	}

    private Regex reg = new Regex(@"^(\S+)\s+(.+)$");
	private void showDialog() {
		clearDialog();
		//string[] tmp = lines[currentLine++].Split ('\t');
	    while (!reg.IsMatch(lines[currentLine]))
	        currentLine++;
	    var tmp = reg.Match(lines[currentLine++]).Groups;

		string character = tmp [1].Value;
		string dialogText = tmp [2].Value;
		dialogBox.GetComponent<Text> ().text = dialogText;
		// Debug.Log ("character is " + character);
		GameObject g = Instantiate (charToFace [character.ToLower()], Vector3.zero, Quaternion.identity) as GameObject;
		g.transform.SetParent(LowerThird.transform);
		RectTransform rtx = g.GetComponent<RectTransform> ();
		rtx.offsetMax = rtx.anchorMax + Vector2.zero;
		rtx.offsetMin = rtx.anchorMin + Vector2.zero;
	}
}
