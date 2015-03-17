using UnityEngine;
using System.Collections;

public class GameOverScreenScript : MonoBehaviour {
	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.activeSelf) {
			if (Input.anyKeyDown) {
				Application.LoadLevel("MainMenu");
			}
		}
	}

	public void show() {
		gameObject.SetActive(true);
	}
}
