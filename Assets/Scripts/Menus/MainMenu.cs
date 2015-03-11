using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenu : MonoBehaviour
{
    public GameObject StartButton;
    public GameObject HeaderOverlay;
    private Vector3 HeaderBase;

    private float LastUpdate;
    public float MoveDelay, Wobble;

    void Start()
    {
        StartButton.GetComponent<Button>().onClick.AddListener(() => { ClickStartGame(); });
        HeaderBase = HeaderOverlay.GetComponent<RectTransform>().position;
    }

    void ClickStartGame()
    {
        Application.LoadLevel("GeneratedLevel");
    }

    void Update()
    {
        var curTime = Time.time;
        if (curTime - LastUpdate > MoveDelay)
        {
            HeaderOverlay.GetComponent<RectTransform>().position = new Vector3(HeaderBase.x + Random.Range(-Wobble, Wobble), HeaderBase.y + Random.Range(-Wobble, Wobble));
            LastUpdate = curTime;
        }
    }
}
