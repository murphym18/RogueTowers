using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{

    public Sprite OpenSprite;
    public int PointsPerChest;
    private bool Opened = false;

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
        }
    }
}
