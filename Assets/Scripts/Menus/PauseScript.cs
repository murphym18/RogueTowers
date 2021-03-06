﻿using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour
{
    public GameObject hud;
    private GradualValue gradualPause;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (gameObject.activeSelf)
        {
			Time.timeScale = gradualPause.TickValue;
            if (Input.GetButtonDown("Quit"))
            {
                Application.LoadLevel("MainMenu");
            }
            else if (Input.anyKeyDown)
            {
                hud.GetComponent<HUD>().show();
                gameObject.SetActive(false);
            }
        }
    }

    public void show()
    {
        gradualPause = new GradualValue(1, 0, 30);
        gameObject.SetActive(true);
    }
}
