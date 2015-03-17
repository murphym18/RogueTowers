using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenu : MonoBehaviour
{
    public GameObject StartButton;

    void Start()
    {
        StartButton.GetComponent<Button>().onClick.AddListener(() => { ClickStartGame(); });
        
    }

    void ClickStartGame()
    {
        Application.LoadLevel("GeneratedLevel");
    }
}
