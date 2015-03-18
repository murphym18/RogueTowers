using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RookBulletScript : BulletScript {
	Collider2D[] enemies;
	float enemyToAttackDistance = 3.0f;
	float lastAttack = 0.0f;
	int enemyToAttackIndex = 0;
	float attackRadius = 10.0f;
    private float bulletYOffset = 0f;
	public LayerMask whatIsTargetable;
	Vector2 distanceVector;

    public override Vector2 velocity
    {
        get { return distanceVector; }
        set
        {
            distanceVector = value;
            var r = gameObject.GetComponent<RectTransform>();
            r.pivot = new Vector2(0, r.rect.height / 2);
            //r.offsetMin = Vector2.zero;
            //r.offsetMax = new Vector2(distanceVector.x, r.rect.height);
            var ang = Vector3.Angle(new Vector3(1, 0), value);
            Debug.Log(ang);
            r.Rotate(Vector3.back, ang);
			//base.velocity = value;
        }
    }

	void Update() {
		//transform.position = new Vector2 (transform.position.x + 1, transform.position.y);
	}


}