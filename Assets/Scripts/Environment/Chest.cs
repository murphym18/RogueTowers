using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Bullet")
        {
            // stop and attack
            int tempBulletDamage = 1;
            this.health -= tempBulletDamage;
            if (this.health <= 0)
                Destroy(gameObject, 0.0f);
        }
    }
}
