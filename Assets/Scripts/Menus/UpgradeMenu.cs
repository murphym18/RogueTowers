using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{

    public GameObject hud;
    public GameObject closeUpgradeScreen;
    public GameObject upgradePointLabel;
    public GameObject gameManager;

    private Tyrem player;

    private int lastKnownUpgradePoints = -1;

	// Use this for initialization
	void Start () {
	    this.gameObject.SetActive(false);
        closeUpgradeScreen.GetComponent<Button>().onClick.AddListener(this.OnClick_CloseUpgradeScreen);
	    player = gameManager.GetComponent<GameManager>().playerInstance.GetComponent<Tyrem>();
        lastKnownUpgradePoints = player.upgradePoints;
	}

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if(Input.GetButtonDown("Cancel"))
                OnClick_CloseUpgradeScreen();
            if (lastKnownUpgradePoints != player.upgradePoints)
                Prepare();
            lastKnownUpgradePoints = player.upgradePoints;
        }
    }

    private void OnClick_CloseUpgradeScreen()
    {
        Time.timeScale = 1;
        this.gameObject.SetActive(false);
        hud.SetActive(true);
    }

    public void show()
    {
        Time.timeScale = 0;
        Prepare();
        gameObject.SetActive(true);
    }

    public void Prepare()
    {
        upgradePointLabel.GetComponent<Text>().text = "Upgrade Points: " +
                                                      gameManager.GetComponent<GameManager>()
                                                          .playerInstance.GetComponent<Tyrem>()
                                                          .upgradePoints;

        _recursivePrepare(this.gameObject);
    }

    private void _recursivePrepare(GameObject obj)
    {
        var button = obj.GetComponent<UpgradeButton>();
		var playerButton = obj.GetComponent<PlayerUpgradeButton>();

        if (button != null)
        {
            button.Prepare();
        }
		else if (playerButton != null)
		{
			playerButton.Prepare();
		}
        foreach (Transform child in obj.transform)
        {
            _recursivePrepare(child.gameObject);
        }
    }
}
