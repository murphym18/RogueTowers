using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WavePanelScript : MonoBehaviour {

	public GameObject textObject;

	private WaveManagerScript waveManager;
	private Text waveText;

	void Awake() {
		waveManager = GameObject.Find("GameManager").GetComponent<WaveManagerScript>();
	}
	
	void Start () {
		waveText = textObject.GetComponent<Text>();
	}

	public void UpdatePanel()
	{
		waveText.text = GetWaveString();
	}

	private string GetWaveString()
	{
		string waveNumber = "Wave:\t\t\t\t" + waveManager.waveNumber.ToString();
		string remaining = "Enemies left:\t" + waveManager.curEnemyCount + "\n";
		string basic = "\t\t\t\tx" + WaveManagerScript.enemiesRemaining[Enemy.EnemyType.Basic].ToString();
		string fast = "\t\t\t\tx" + WaveManagerScript.enemiesRemaining[Enemy.EnemyType.Fast].ToString();
		string strong = "\t\t\t\tx" + WaveManagerScript.enemiesRemaining[Enemy.EnemyType.Strong].ToString();

		string[] arr = new string[] {waveNumber,remaining,basic,fast,strong};

		return string.Join("\n", arr);
	}
}
