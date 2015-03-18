using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RookBulletScript : BulletScript
{
    private Collider2D[] enemies;
    private float enemyToAttackDistance = 3.0f;
    private float lastAttack = 0.0f;
    private int enemyToAttackIndex = 0;
    private float attackRadius = 10.0f;
    public LayerMask whatIsTargetable;
    private Vector2 distanceVector;

    public override Vector2 velocity
    {
        get { return distanceVector; }
        set
        {
            distanceVector = value;
            var r = gameObject.GetComponent<RectTransform>();
            r.pivot = new Vector2(0, r.rect.height/2);
            r.offsetMax = new Vector2(0.5f + r.offsetMin.x + (r.offsetMax.x - r.offsetMin.x)*distanceVector.magnitude, r.offsetMax.y);

            this.GetComponent<BoxCollider2D>().size = new Vector2(distanceVector.magnitude, 1);
            
            this.collider2D.transform.localScale = new Vector3(distanceVector.magnitude, 1, 1);

            //r.offsetMin = new Vector2();
            //r.offsetMax = new Vector2(distanceVector.x, r.rect.height);
            var ang = Vector3.Angle(new Vector3(1, 0), value);
            var norm = Vector3.Cross(new Vector3(1, 0), value).normalized;
            r.Rotate(norm, ang);
        }
    }

    private void Update()
    {
        //transform.position = new Vector2 (transform.position.x + 1, transform.position.y);
    }

    private new void OnTriggerEnter2D(Collider2D coll)
    {
        // Just prevent the parent class from destroying this bullet
    }

}