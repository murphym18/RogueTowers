using UnityEngine;
using System.Collections;

public class LoadingScreenScript : MonoBehaviour {
	public string gameWorldLevel = "GeneratedLevel";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Application.LoadLevel(gameWorldLevel);
	}
}
