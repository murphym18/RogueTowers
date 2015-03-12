using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public GameObject OpenUpgradeScreen;
    public GameObject UpgradeScreen;

	public GameObject[] towerButtons;

	// Use this for initialization
	void Start () {
	    OpenUpgradeScreen.GetComponent<Button>().onClick.AddListener(this.OnClick_OpenUpgradeScreen);

		foreach (GameObject button in towerButtons)
			AddTowerButton(button);
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
			//button.GetComponent<TowerButtonScript>().Prepare();
		}
	}

	public void AddTowerButton(GameObject towerButton)
	{
		//towerButton.GetComponent<TowerButtonScript>().Initialize();
	}
}
