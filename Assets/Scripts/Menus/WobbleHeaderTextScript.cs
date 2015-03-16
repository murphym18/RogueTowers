using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WobbleHeaderTextScript : MonoBehaviour {

	public string foregroundGameObjectName = "ForegroundText";
	public string backgroundGameObjectName = "BackgroundText";

	private GameObject foregroundText = null;
	private RectTransform foregroundTx = null;
	private Vector3 foregroundPos;

	private GameObject backgroundText = null;
	private RectTransform backgroundTx = null;
	private Vector3 backgroundPos;
	private Vector3 backgroundOffset;
	public float backgroundXOffset = 3F;
	public float backgroundYOffset = -3F;


	private float LastUpdate = 0F;
	public float MoveDelay = 0.2F;
	public float Wobble = 2F;
	// Use this for initialization
	void Start(){
		backgroundOffset = new Vector3(backgroundXOffset, backgroundYOffset, 0);
		backgroundPos = new Vector3(backgroundXOffset, backgroundYOffset, 0);
		foreach(RectTransform tx in gameObject.GetComponentsInChildren<RectTransform>()) {
			string gameobjName = tx.gameObject.name;
			if (gameobjName == foregroundGameObjectName) {
				foregroundText = tx.gameObject;
				foregroundTx = tx;
				foregroundPos = foregroundTx.position;
			}
			else if (gameobjName == backgroundGameObjectName) {
				backgroundText = tx.gameObject;
				backgroundTx = tx;
				backgroundPos = backgroundTx.position +  backgroundOffset;
			}
			if (foregroundText != null && backgroundText != null) {
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		var curTime = Time.time;
		if (curTime - LastUpdate > MoveDelay) {


			Vector3 randOffset = new Vector3(Random.Range(-Wobble, Wobble), Random.Range(-Wobble, Wobble), 0);
			foregroundTx.position = foregroundPos + randOffset;
			backgroundTx.position = backgroundPos + randOffset;
			LastUpdate = curTime;
		}
	}
}
