﻿using UnityEngine;
using System.Collections;

public class StartMenuScript : MonoBehaviour {

	public string mainMenu = "MainMenu";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Application.LoadLevel(mainMenu);
		}
	}
}
