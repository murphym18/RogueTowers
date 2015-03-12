using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class UpgradeButton : MonoBehaviour
{

    public enum UpgradeType { IncreaseStats, AddExtras }

    public UpgradeType upgradeType;
    public TestTowerScript.TowerType towerType;

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

        costLabel.GetComponent<Text>().text = cost.ToString();
        costLabel.GetComponent<Text>().color = isBuyable ? sufficientUpgradePoints : insufficientUpgradePoints;
        this.GetComponent<Button>().interactable = isBuyable;
    }

    int CalculateUpgradeCost()
    {
        return 1;
    }

    void OnClick()
    {
        if (isBuyable)
        {
            switch (upgradeType)
            {
                case UpgradeType.AddExtras:
                    TowerPlacement.ExtraTowers[towerType] += 1;
                    Debug.Log("" + Enum.GetName(typeof(TestTowerScript.TowerType), towerType) + " just got more plentiful!");
                    break;

                case UpgradeType.IncreaseStats:
                    TestTowerScript.UpgradeLevels[towerType] += 1;
                    Debug.Log("" + Enum.GetName(typeof(TestTowerScript.TowerType), towerType) + " just got more powerful!");
                    break;
            }
            player.deductUpgradePoints(cost);
            GameObject.Find("UpgradeMenu").GetComponent<UpgradeMenu>().Prepare();
        }
    }
}
