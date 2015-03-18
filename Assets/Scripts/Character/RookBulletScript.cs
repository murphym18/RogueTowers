using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RookBulletScript : BulletScript
{
    private Vector2 distanceVector;

    public override Vector2 velocity
    {
        get { return distanceVector; }
        set
        {
            distanceVector = value;
            var r = gameObject.GetComponent<RectTransform>();
            var width = value.magnitude + 0.5f;

            r.pivot = new Vector2(0, r.rect.height/2);
            r.offsetMax = new Vector2(r.offsetMin.x + (r.offsetMax.x - r.offsetMin.x)*width, r.offsetMax.y);

            var coll = this.GetComponent<BoxCollider2D>();
            coll.size = new Vector2(width, r.rect.height);
            coll.center = new Vector2(width / 2, r.rect.height / 5f); //  Why /5 ? I don't know...

            //r.offsetMin = new Vector2();
            //r.offsetMax = new Vector2(distanceVector.x, r.rect.height);
            var ang = Vector3.Angle(new Vector3(1, 0), value);
            var norm = Vector3.Cross(new Vector3(1, 0), value).normalized;
            r.Rotate(norm, ang);
        }
    }

    private new void OnTriggerEnter2D(Collider2D coll)
    {
        // Just prevent the parent class from destroying this bullet
    }

}