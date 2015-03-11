using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerType;
    public GameObject PlayerInstance;
	public int currentLevel = 0;
    private BoardManager BoardScript;
	
	// Use this for initialization
	void Awake ()
	{
	    BoardScript = GetComponent<BoardManager>();
	    InitGame();
	    PlayerInstance = (GameObject)Instantiate(PlayerType, new Vector3(0, (int)(BoardScript.levelHeight/2)), Quaternion.identity);
	}

    private void InitGame()
    {
        BoardScript.SetupScene();
    }
	
	// Update is called once per frame
	void Update ()
	{
	}
}
