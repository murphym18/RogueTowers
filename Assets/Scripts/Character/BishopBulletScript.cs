using System;
using UnityEngine;
using System.Collections.Generic;
public class BishopBulletScript : BulletScript {
	Collider2D[] enemies;
	float enemyToAttackDistance = 3.0f;
	float lastAttack = 0.0f;
	int enemyToAttackIndex = 0;
	float attackRadius = 10.0f;
	public LayerMask whatIsTargetable;

    private bool flippedX, flippedY;

	BishopBulletScript() {
		damage = 7.0f;
		speed = 9.0f;
	}

	void Start()
	{
	    base.Start();
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		OnTriggerEnter2D(coll.collider);
	}

    void FixedUpdate()
    {
        flippedX = flippedY = false;
    }

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (ThingsToDieOn.Contains(coll.gameObject.tag) && coll.gameObject.tag == "Enemy")
		{
			//Explosion!!!
			Destroy(gameObject, 0.0f);
		} else if (ThingsToDieOn.Contains(coll.gameObject.tag) && coll.gameObject.tag == "Cage" || coll.gameObject.tag == "Wall"){
			//Bounce!!!
			Bounce (coll);
		}
	}

    void Bounce(Collider2D coll)
	{
	    var collX = transform.position.x - coll.transform.position.x;
        var collY = transform.position.y - coll.transform.position.y;

        if (Math.Abs(collY) > Math.Abs(collX)) // Vertical bounce
        {
            if (!flippedY)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -rigidbody2D.velocity.y);
                if (coll is BoxCollider2D)
                {
                    var signY = collY < 0 ? -1 : 1;
                    var box = (BoxCollider2D) coll;
                    this.transform.position = new Vector2(rigidbody2D.position.x,
                        box.transform.position.y + signY*0.5f*(box.size.y + this.GetComponent<BoxCollider2D>().size.y));
                }
                flippedY = true;
            }
        }
        else // Horizontal bounce
        {
            if (!flippedX)
            {
                rigidbody2D.velocity = new Vector2(-rigidbody2D.velocity.x, rigidbody2D.velocity.y);
                if (coll is BoxCollider2D)
                {
                    var signX = collX < 0 ? -1 : 1;
                    var box = (BoxCollider2D) coll;
                    this.transform.position =
                        new Vector2(
                            box.transform.position.x +
                            signX*0.5f*(box.size.x + this.GetComponent<BoxCollider2D>().size.x), rigidbody2D.position.y);
                }
                flippedX = true;
            }
        }
        rigidbody2D.position = this.transform.position;
	}
}