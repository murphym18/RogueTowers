using UnityEngine;
using System.Collections;

public class TestTowerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy(gameObject, 0.7777777f);
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody2D.velocity = new Vector3(0, 5, 0);
	}
}
