using UnityEngine;
using System.Collections;
using Helpers;

public class Chest : IsometricObject
{

    public Sprite OpenSprite;
    public int PointsPerChest;
    private bool Opened = false;

    private static bool firstMessage = true;

    private string message
    {
        get
        {
            if (firstMessage)
            {
                firstMessage = false;
                return "{0} Upgrade points (UP)".QuickFormat(PointsPerChest);
            }
            return "{0} UP".QuickFormat(PointsPerChest);
        }
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!Opened && coll.gameObject.tag == "Player")
        {
            Opened = true;
            GetComponent<SpriteRenderer>().sprite = OpenSprite;
            coll.gameObject.GetComponent<Tyrem>().addUpgradePoints(PointsPerChest);
            GameObject.Find("GameManager").GetComponent<GameManager>().DisplayMessage(message);
        }
    }
}
