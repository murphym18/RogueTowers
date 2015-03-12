using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;

public class UpgradeButton : MonoBehaviour
{

    public enum UpgradeType { IncreaseStats, AddExtras }

    public UpgradeType upgradeType;
    public TestTowerScript.TowerType towerType;

    public GameObject levelLabel;
    public GameObject costLabel;
    public GameObject gameManagerObject;
    private Tyrem player;
    private int cost;

    private bool isBuyable;
    public Color sufficientUpgradePoints, insufficientUpgradePoints;

    // Use this for initialization
	void Start () {
	    this.GetComponent<Button>().onClick.AddListener(this.OnClick);
	    player = gameManagerObject.GetComponent<GameManager>().PlayerInstance.GetComponent<Tyrem>();
	}

    public void Prepare()
    {
        cost = CalculateUpgradeCost();
        isBuyable = player.upgradePoints >= cost;

        levelLabel.GetComponent<Text>().text = Table[towerType].ToString();

        costLabel.GetComponent<Text>().text = cost.ToString();
        costLabel.GetComponent<Text>().color = isBuyable ? sufficientUpgradePoints : insufficientUpgradePoints;
        this.GetComponent<Button>().interactable = isBuyable;

        if(TowerPlacement.ExtraTowers[towerType] == -1)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    int CalculateUpgradeCost()
    {
        var curVal = Table[towerType] + 1;
        switch (upgradeType)
        {
            case UpgradeType.AddExtras:
                switch (towerType)
                {
                    case TestTowerScript.TowerType.Pawn:
                        return 5 * curVal;
                    case TestTowerScript.TowerType.Knight:
                        return 10 * curVal;
                    case TestTowerScript.TowerType.Bishop:
                        return 10 * curVal;
                    case TestTowerScript.TowerType.Rook:
                        return 10 * curVal;
                    case TestTowerScript.TowerType.King:
                        return 20 * curVal;
                    case TestTowerScript.TowerType.Queen:
                        return 20 * curVal;
                }
                break;

            case UpgradeType.IncreaseStats:
                switch (towerType)
                {
                    case TestTowerScript.TowerType.Pawn:
                        return 2 * curVal;
                    case TestTowerScript.TowerType.Knight:
                        return 3 * curVal;
                    case TestTowerScript.TowerType.Bishop:
                        return 3 * curVal;
                    case TestTowerScript.TowerType.Rook:
                        return 3 * curVal;
                    case TestTowerScript.TowerType.King:
                        return 5 * curVal;
                    case TestTowerScript.TowerType.Queen:
                        return 5 * curVal;
                }
                break;
        }
        throw new InvalidOperationException("Invalid type of upgrade");
    }

    private Dictionary<TestTowerScript.TowerType, int> Table
    {
        get
        {
            switch (upgradeType)
            {
                case UpgradeType.AddExtras:
                    return TowerPlacement.ExtraTowers;
                case UpgradeType.IncreaseStats:
                    return TestTowerScript.UpgradeLevels;
                default:
                    throw new InvalidOperationException("Unknown upgrade type: " + upgradeType);
            }
        }
    }

    void OnClick()
    {
        if (isBuyable)
        {
            Table[towerType] += 1;
            player.deductUpgradePoints(cost);
            GameObject.Find("UpgradeMenu").GetComponent<UpgradeMenu>().Prepare();
        }
    }
}
