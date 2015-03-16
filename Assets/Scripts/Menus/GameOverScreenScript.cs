using UnityEngine;
using System.Collections;

public class GameOverScreenScript : MonoBehaviour {
	private bool isVisible = false;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (isVisible) {
			if (Input.anyKeyDown) {
				Application.LoadLevel("MainMenu");
			}
		}
	}

	public void show() {
		gameObject.GetComponent<CanvasGroup>().alpha = 1F;
		isVisible = true;
	}
}
