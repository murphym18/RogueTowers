using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthBarForegroundScript : MonoBehaviour {
	private Image healthBarForeground;
	private GameObject player;
	private Tyrem tyrem;
	private PlayerHealthBarColorsScript healthBarColors;
	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("Player");
		tyrem = player.GetComponent<Tyrem>();
		healthBarForeground = GameObject.Find("/HUD/PlayerHealthBar/HealthBarPanel/HealthBarForeground").GetComponentInChildren<Image>();
		healthBarColors = this.gameObject.GetComponentInParent<PlayerHealthBarColorsScript>();
	}
	
	// Update is called once per frame
	void Update () {
		float max = tyrem.getMaxHp();
		float hp = (float) tyrem.hp;
		float health = Math.Min (Math.Max(hp/max, 0), 1);
		Color healthBarColor = Color.Lerp(healthBarColors.healthFull, healthBarColors.healthEmpty, 1 - health);
		healthBarForeground.color = healthBarColor;
		healthBarForeground.transform.localScale = new Vector3 (health, 1, 1);
	}
}
