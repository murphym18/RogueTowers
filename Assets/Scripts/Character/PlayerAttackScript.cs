using UnityEngine;
using System.Collections;

public class PlayerAttackScript : MonoBehaviour {

	// TODO: implement multiple attacks

	public Rigidbody2D bullet;

	private TowerPlacement towerPlacement;
	private float attackTimeout = 0.4f;
	private float lastAttackTime = 0;

	// Use this for initialization
	void Start () {
		towerPlacement = GetComponent<TowerPlacement>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Attack") && !towerPlacement.IsTowerSelected())
		{
			// fire bullet in mouse cursor location
			if (Time.time > lastAttackTime + attackTimeout)
			{
				lastAttackTime = Time.time;
				ShootBullet();
			}
		}
	}

	private void ShootBullet()
	{
		Vector3 worldLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		worldLocation.z = 0;
		Vector3 velocity = (worldLocation - transform.position).normalized;
		
		Rigidbody2D attackInstance = Instantiate(bullet, transform.position, Quaternion.identity) as Rigidbody2D;
		attackInstance.velocity = velocity;
	}
	
}
