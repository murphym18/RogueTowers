using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerUpgradeButton : MonoBehaviour
{
	
	public enum UpgradeType { RecoverHealth, BasicAttack, SpreadAttack }
	
	public UpgradeType upgradeType;
	public PlayerAttackScript.AttackType attackType;
	
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
	}
	
	public void Prepare()
	{
		player = gameManagerObject.GetComponent<GameManager>().playerInstance.GetComponent<Tyrem>();
		cost = CalculateUpgradeCost();
		isBuyable = player.upgradePoints >= cost;

		if (upgradeType == UpgradeType.RecoverHealth)
		{
			isBuyable = player.upgradePoints >= cost && player.hp < player.getMaxHp();
			levelLabel.GetComponent<Text>().text = "";
		}
		else
		{
			isBuyable = player.upgradePoints >= cost;
			levelLabel.GetComponent<Text>().text = Table[attackType].ToString();
		}
		
		costLabel.GetComponent<Text>().text = cost.ToString();
		costLabel.GetComponent<Text>().color = isBuyable ? sufficientUpgradePoints : insufficientUpgradePoints;
		this.GetComponent<Button>().interactable = isBuyable;
	}
	
	int CalculateUpgradeCost()
	{
		var curVal = 1;
		if (upgradeType != UpgradeType.RecoverHealth)
			curVal = Table[attackType] + 1;

		switch (upgradeType)
		{
		case UpgradeType.RecoverHealth:
			return 10;
		
		case UpgradeType.BasicAttack:
			return 5*curVal;
		
		case UpgradeType.SpreadAttack:
			return 5*curVal;
		}
		throw new InvalidOperationException("Invalid type of upgrade");
	}
	
	private Dictionary<PlayerAttackScript.AttackType, int> Table
	{
		get
		{
			switch (upgradeType)
			{
			case UpgradeType.RecoverHealth:
				return PlayerAttackScript.PlayerAttackLevels;
			case UpgradeType.BasicAttack:
				return PlayerAttackScript.PlayerAttackLevels;
			case UpgradeType.SpreadAttack:
				return PlayerAttackScript.PlayerAttackLevels;
			default:
				throw new InvalidOperationException("Unknown upgrade type: " + upgradeType);
			}
		}
	}
	
	void OnClick()
	{
		if (isBuyable)
		{
			if (upgradeType == UpgradeType.RecoverHealth)
				player.hp = player.getMaxHp();
			else
				Table[attackType] += 1;
			player.deductUpgradePoints(cost);
			GameObject.Find("UpgradeMenu").GetComponent<UpgradeMenu>().Prepare();
		}
	}
}
