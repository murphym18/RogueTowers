using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TowerButtonScript : MonoBehaviour {

	public TestTowerScript.TowerType towerType;
	public GameObject numberLabel;
	public GameObject levelLabel;
	public GameObject countLabel;
	public GameObject towerTypeImage;

	private int numberKey;

	private GameManager gameManager;
	private TowerPlacement towerPlacement;

    private int prevRemaining;
	
	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponentInParent<GameManager>();
	}

	public void Initialize(TestTowerScript.TowerType setType)
	{
		towerType = setType;
		towerTypeImage.GetComponent<Image>().sprite = HUD.towerSpriteDict[towerType];
		towerPlacement = gameManager.playerInstance.GetComponent<TowerPlacement>();
		numberKey = TowerPlacement.TowerKeys[towerType];
		GetComponent<Button>().onClick.AddListener(this.OnClick_SelectTower);
	}

	public void OnClick_SelectTower()
	{
		towerPlacement.SelectTower(towerType);
	}

    private int Remaining
    {
        get { return (TowerPlacement.ExtraTowers[towerType] - TowerPlacement.PlacedTowers[towerType]); }
    }

    public void Prepare()
	{
		string level = TestTowerScript.UpgradeLevels[towerType].ToString();

		numberLabel.GetComponent<Text>().text = numberKey.ToString();
		//levelLabel.GetComponent<Text>().text = "lv. " + level;
		countLabel.GetComponent<Text>().text = "x" + Remaining.ToString();

        prevRemaining = Remaining;
	}

    void Update()
    {
        if (Remaining != prevRemaining)
            countLabel.GetComponent<Text>().text = "x" + Remaining.ToString();
    }

}
