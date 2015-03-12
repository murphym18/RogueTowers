using UnityEngine;
using System.Collections;

public class FloorTileScript : MonoBehaviour {

	Animator anim;
	public int floorTileVersion = 1; //1 through 4
	
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		floorTileVersion = Random.Range (1, 4);
		//anim.SetInteger ("FloorTileVersion", floorTileVersion);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
