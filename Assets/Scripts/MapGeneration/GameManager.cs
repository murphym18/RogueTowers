using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject playerType;
    public GameObject playerInstance;
    public GameObject gameOverScreen;
	public int currentLevel = -1;
    private BoardManager BoardScript;

    public GameObject messageBoardObject;
    private MessageBoardScript msgBoard;
	
	// Use this for initialization
	void Awake ()
	{
	    Time.timeScale = 1;
        msgBoard = messageBoardObject.GetComponent<MessageBoardScript>();

        BoardScript = GetComponent<BoardManager>();
        BoardScript.SetupScene();
	    playerInstance = (GameObject)Instantiate(playerType, new Vector3(0, (int)(BoardScript.levelHeight/2)), Quaternion.identity);
	    playerInstance.GetComponent<Tyrem>().gameOverScreen = gameOverScreen;

        TowerPlacement.Initialize();
        TestTowerScript.Initialize();
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

    public void DisplayMessage(string Msg)
    {
        msgBoard.DisplayMessage(Msg.ToUpper());
    }
}
