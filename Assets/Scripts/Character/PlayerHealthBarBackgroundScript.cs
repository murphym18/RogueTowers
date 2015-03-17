using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthBarBackgroundScript : MonoBehaviour {
	private Image healthBarBackground;
	private GameObject player;
	private Tyrem tyrem;
	private PlayerHealthBarColorsScript healthBarColors;
	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("Player");
		tyrem = player.GetComponent<Tyrem>();
		healthBarBackground = GameObject.Find("/HUD/HealthBarPanel/HealthBarBackground").GetComponentInChildren<Image>();
		healthBarColors = this.gameObject.GetComponentInParent<PlayerHealthBarColorsScript>();
	}
	
	// Update is called once per frame
	void Update () {
		float max = tyrem.getMaxHp();
		float hp = (float) tyrem.hp;
		float health = hp/max;
		Color healthBarColor = Color.Lerp(healthBarColors.healthFull, healthBarColors.healthEmpty, 1 - health);
		healthBarBackground.color = healthBarColor;
	}
}
