using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public GameObject OpenUpgradeScreen;
    public GameObject UpgradeScreen;
	public GameObject towerButton;
	public List<GameObject> towerButtons = new List<GameObject>();

	private float nextAnchorX = 0.07f;
	private float towerButtonX = 0.0636f;
	private float towerButtonY = 0.1178011f;

	public static Dictionary<TestTowerScript.TowerType, Sprite> towerSpriteDict = new Dictionary<TestTowerScript.TowerType, Sprite>();
	public Sprite pawnSprite;
	public Sprite knightSprite;
	public Sprite bishopSprite;
	public Sprite rookSprite;
	public Sprite kingSprite;
	public Sprite queenSprite;
	
	// Use this for initialization
	void Start () {
	    OpenUpgradeScreen.GetComponent<Button>().onClick.AddListener(this.OnClick_OpenUpgradeScreen);

		InitializeSprites();

		// Temporarily start the player with all towers just for testing purposes
		foreach (TestTowerScript.TowerType towerType in Enum.GetValues(typeof (TestTowerScript.TowerType)))
			TowerPlacement.AddTowerType(towerType);
		PrepareChildren();
	}

    private void OnClick_OpenUpgradeScreen()
    {
        this.gameObject.SetActive(false);
        UpgradeScreen.GetComponent<UpgradeMenu>().Prepare();
        UpgradeScreen.SetActive(true);
    }

	public void PrepareChildren()
	{
		foreach (GameObject button in towerButtons)
		{
			button.GetComponent<TowerButtonScript>().Prepare();
		}
	}

	public void AddTowerButton(TestTowerScript.TowerType towerType)
	{
		GameObject buttonInstance = (GameObject) Instantiate(towerButton);
		buttonInstance.transform.SetParent(this.transform);

		RectTransform rect = buttonInstance.GetComponent<RectTransform>();
		rect.anchorMin = new Vector2(nextAnchorX, 0);
		rect.anchorMax = new Vector2(nextAnchorX + towerButtonX, towerButtonY);
		rect.offsetMin = new Vector2(rect.offsetMin.x, -1);
		rect.offsetMax = new Vector2(rect.offsetMax.x, -5);

		nextAnchorX += towerButtonX;
		
		buttonInstance.GetComponent<TowerButtonScript>().Initialize(towerType);
		towerButtons.Add(buttonInstance);
	}

	private void InitializeSprites()
	{
		towerSpriteDict.Add(Tower("Pawn"), pawnSprite);
		towerSpriteDict.Add(Tower("Knight"), knightSprite);
		towerSpriteDict.Add(Tower("Bishop"), bishopSprite);
		towerSpriteDict.Add(Tower("Rook"), rookSprite);
		towerSpriteDict.Add(Tower("King"), kingSprite);
		towerSpriteDict.Add(Tower("Queen"), queenSprite);
	}
	
	private static TestTowerScript.TowerType Tower(string towerType)
	{
		return (TestTowerScript.TowerType)Enum.Parse(typeof(TestTowerScript.TowerType), towerType);
	}
}
