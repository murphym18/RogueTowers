using UnityEngine;
using System.Collections.Generic;
public abstract class BulletScript : MonoBehaviour {


	public List<string> ThingsToDieOn = new List<string>();
	public float damage = 1.0f;
	public float speed = 3.0f;
    public float lifetime = 6f;

    public virtual Vector2 velocity
    {
        get { return this.rigidbody2D.velocity; }
        set { this.rigidbody2D.velocity = value.normalized*speed; }
    }

	// Use this for initialization
	protected void Start () {
		Destroy (gameObject, lifetime);
	}

	protected void OnTriggerEnter2D(Collider2D coll)
	{
		if (ThingsToDieOn.Contains(coll.gameObject.tag))
		{
			Destroy(gameObject, 0f);
		}
	}
}