using UnityEngine;
using System.Collections;

public class LoadingScreenScript : MonoBehaviour {
	public string gameWorldLevel = "GeneratedLevel";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Application.GetStreamProgressForLevel(gameWorldLevel) == 1){
			Application.LoadLevel(gameWorldLevel);
		}

	}
}
