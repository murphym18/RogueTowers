using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Cage : MonoBehaviour {

	public int hp;
	public float unlockDistance;
	public float unlockTime;

	private GameObject gameManager;
	private GameObject player;
	private float progress = 0F;
	private bool isUnlocked = false;

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find("GameManager");
		player = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (!isUnlocked) {
			if (Input.GetButton ("UnlockCage")) {
				Vector3 d = this.transform.position - player.transform.position;

				if (d.sqrMagnitude < unlockDistance*unlockDistance) {
					progress += Time.deltaTime;
					if (unlockTime < progress) {
						isUnlocked = true;
						Destroy(gameObject, 0);

					}
				}
			}
		}
	}

	public void damage() {
		if (hp > 0) {
			hp -= 1;
		} else {

		}
	}
}
