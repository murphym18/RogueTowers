using UnityEngine;
using System.Collections;

public class StoryScreenScript : MonoBehaviour {
	public GameObject hud;
	public GameObject levelTransitionNotice;

	void Start(){
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.activeSelf) {
			Time.timeScale = 0;
			if (Input.anyKeyDown) {
				hud.GetComponent<HUD>().show();
				gameObject.SetActive(false);
				levelTransitionNotice.SetActive(true);
				Destroy(levelTransitionNotice, 3.9f );
			}
		}
	}

	public void show(int levelNumber, GameObject levelTransitionNotice) {
		this.levelTransitionNotice = levelTransitionNotice;
		Debug.Log ("started level " + levelNumber);
		gameObject.SetActive(true);
	}

	public void resume() {
		hud.GetComponent<HUD>().show();
		gameObject.SetActive(false);
	}
}
