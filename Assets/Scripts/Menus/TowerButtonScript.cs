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
	private GameObject towerTooltip;
	private Text tooltipText;
	private float tooltipTimeout = 0.4f;
	private float tooltipTimer = 0;
	private bool tooltipTimerStarted = false;

    private int prevRemaining;
	
	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponentInParent<GameManager>();
	}

	public void Initialize(TestTowerScript.TowerType setType, GameObject tooltip)
	{
		towerType = setType;
		towerTypeImage.GetComponent<Image>().sprite = HUD.towerSpriteDict[towerType];
		towerPlacement = gameManager.playerInstance.GetComponent<TowerPlacement>();
		numberKey = TowerPlacement.TowerKeys[towerType];
		towerTooltip = tooltip;
		towerTooltip.SetActive(true);
		tooltipText = towerTooltip.GetComponentInChildren<Text>();
		towerTooltip.SetActive(false);
		GetComponent<Button>().onClick.AddListener(this.OnClick_SelectTower);
	}

	public void OnClick_SelectTower()
	{
		towerPlacement.SelectTower(towerType);
	}

    private int Remaining
    {
        get { return (TowerPlacement.TowerCount[towerType] - TowerPlacement.PlacedTowers[towerType]); }
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
		if (tooltipTimerStarted && tooltipTimer > 0)
		{
			if ((tooltipTimer -= Time.deltaTime) <= 0)
				ShowTooltip();
		}
    }

	public void StartTooltipTimer()
	{
		tooltipTimer = tooltipTimeout;
		tooltipTimerStarted = true;
	}

	public void ShowTooltip()
	{
		towerTooltip.transform.position = transform.position;
		towerTooltip.transform.Translate(0, GetComponent<RectTransform>().rect.height/2, 0);
		tooltipText.text = GetTooltipString();
		towerTooltip.SetActive(true);
	}

	public void HideTooltip()
	{
		towerTooltip.SetActive(false);
		tooltipTimerStarted = false;
	}

	private string GetTooltipString()
	{
		string type = towerType.ToString();
		string level = "level: " + TestTowerScript.UpgradeLevels[towerType].ToString();

		return type + "\n" + level;
	}
}
