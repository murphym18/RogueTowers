using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public BoardManager BoardScript;
    private int Level = 3;


	// Use this for initialization
	void Awake ()
	{
	    BoardScript = GetComponent<BoardManager>();
	    InitGame();
	}

    private void InitGame()
    {
        BoardScript.SetupScene();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
