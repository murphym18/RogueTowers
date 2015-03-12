using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;

public class TowerButtonScript : MonoBehaviour {

	public TestTowerScript.TowerType towerType;
	public GameObject numberLabel;
	public GameObject levelLabel;
	public GameObject countLabel;
	public int numberKey;

	private GameManager gameManager;
	private TowerPlacement towerPlacement;
	private enum stat { IncreaseStats, AddExtras }

	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponentInParent<GameManager>();
	}

	public void Initialize()
	{
		towerPlacement = gameManager.PlayerInstance.GetComponent<TowerPlacement>();
		GetComponent<Button>().onClick.AddListener(this.OnClick_SelectTower);
	}

	public void OnClick_SelectTower()
	{
		towerPlacement.SelectTower(towerType);
	}

	public void Prepare()
	{
		string level = TestTowerScript.UpgradeLevels[towerType].ToString();
		string count = (TowerPlacement.ExtraTowers[towerType] - TowerPlacement.PlacedTowers[towerType]).ToString();

		numberLabel.GetComponent<Text>().text = numberKey.ToString();
		//levelLabel.GetComponent<Text>().text = "lv. " + level;
		countLabel.GetComponent<Text>().text = "x" + count;
	}

}
