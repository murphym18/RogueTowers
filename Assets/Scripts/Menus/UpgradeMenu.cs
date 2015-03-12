using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{

    public GameObject hud;
    public GameObject closeUpgradeScreen;
    public GameObject upgradePointLabel;
    public GameObject gameManager;

	// Use this for initialization
	void Start () {
	    this.gameObject.SetActive(false);
        closeUpgradeScreen.GetComponent<Button>().onClick.AddListener(this.OnClick_CloseUpgradeScreen);
	}

    private void OnClick_CloseUpgradeScreen()
    {
        this.gameObject.SetActive(false);
        hud.SetActive(true);
    }

    public void Prepare()
    {
        upgradePointLabel.GetComponent<Text>().text = "Upgrade Points: " +
                                                      gameManager.GetComponent<GameManager>()
                                                          .PlayerInstance.GetComponent<Tyrem>()
                                                          .upgradePoints;

        _recursivePrepare(this.gameObject);
    }

    private void _recursivePrepare(GameObject obj)
    {
        var button = obj.GetComponent<UpgradeButton>();
        if (button != null)
        {
            button.Prepare();
        }
        foreach (Transform child in obj.transform)
        {
            _recursivePrepare(child.gameObject);
        }
    }
}
