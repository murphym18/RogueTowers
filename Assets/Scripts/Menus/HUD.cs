using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public GameObject OpenUpgradeScreen;
    public GameObject UpgradeScreen;

	// Use this for initialization
	void Start () {
	    OpenUpgradeScreen.GetComponent<Button>().onClick.AddListener(this.OnClick_OpenUpgradeScreen);
	}

    private void OnClick_OpenUpgradeScreen()
    {
        this.gameObject.SetActive(false);
        UpgradeScreen.GetComponent<UpgradeMenu>().Prepare();
        UpgradeScreen.SetActive(true);
    }
}
