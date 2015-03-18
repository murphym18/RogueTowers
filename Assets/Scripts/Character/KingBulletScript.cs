using UnityEngine;
using System.Collections.Generic;
public class KingBulletScript : BulletScript
{
    public float minSize;
    private float startTime, endTime, maxSize;
    private CircleCollider2D coll;

    public override Vector2 velocity
    {
        get { return Vector2.zero; }
        set { }
    }

    void Start()
    {
        base.Start();
        maxSize = parent.attackRadius * 2;
        startTime = Time.time;
        coll = this.GetComponent<CircleCollider2D>();
        Update();
    }

    void Update()
    {
        var newScale = minSize + (maxSize - minSize)*((Time.time - startTime)/lifetime);
        this.transform.localScale = new Vector3(newScale, newScale, 1);
    }
}