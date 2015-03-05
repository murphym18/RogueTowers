using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{

    public Sprite OpenSprite;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            GetComponent<SpriteRenderer>().sprite = OpenSprite;
        }
    }
    */
}
