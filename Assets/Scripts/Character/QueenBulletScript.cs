using UnityEngine;
using System.Collections.Generic;
public class QueenBulletScript : BulletScript
{
    private Vector2 distanceVector;
    private SpriteRenderer spriteRenderer;
    private float startTime;

    public override Vector2 velocity
    {
        get { return distanceVector; }
        set
        {
            distanceVector = value;
            var r = gameObject.GetComponent<RectTransform>();
            var width = value.magnitude;

            r.pivot = new Vector2(0, r.rect.height / 2);
            r.offsetMax = new Vector2(r.offsetMin.x + (r.offsetMax.x - r.offsetMin.x) * width, r.offsetMax.y);

            var coll = this.GetComponent<BoxCollider2D>();
            //coll.size = new Vector2(width, r.rect.height);
            coll.center = new Vector2(r.rect.width - coll.size.x / 2, r.rect.height / 3f); //  Why /3 ? I don't know...

            //r.offsetMin = new Vector2();
            //r.offsetMax = new Vector2(distanceVector.x, r.rect.height);
            var ang = Vector3.Angle(new Vector3(1, 0), value);
            var norm = Vector3.Cross(new Vector3(1, 0), value).normalized;
            r.Rotate(norm, ang);
        }
    }

    void Start()
    {
        startTime = Time.time;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        base.Start();
    }

    void Update()
    {
        var c = spriteRenderer.color;
        spriteRenderer.color = new Color(c.r, c.g, c.b, 1 - (Time.time - startTime) / lifetime);
    }

    private new void OnTriggerEnter2D(Collider2D coll)
    {
        this.collider2D.enabled = false;
    }
}