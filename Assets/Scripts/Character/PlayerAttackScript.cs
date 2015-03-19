using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttackScript : MonoBehaviour {

	// TODO: implement multiple attacks

	public Rigidbody2D basicBullet;
	public Rigidbody2D spreadBullet;

	public float damageUpgradeMultiplier = 0.25f;

	private TowerPlacement towerPlacement;

	private float basicBulletSpeed = 14f;
	private float basicBulletDelay = 0.4f;
	private float basicBulletTime = 0;

	private float spreadBulletSpeed = 10f;
	private float spreadBulletDelay = 1.0f;
	private float spreadBulletTime = 0;

	public enum AttackType {
		BasicBullet,
		SpreadBullet
	}
	public static readonly Dictionary<AttackType, int> PlayerAttackLevels = new Dictionary<AttackType, int>();
	private AttackType[] attackTypeList = {AttackType.BasicBullet, AttackType.SpreadBullet};
	private int curAttackTypeIndex = 0;
	private GameObject gameManager;
	public static void Initialize()
	{
		foreach (AttackType attackType in Enum.GetValues(typeof (AttackType))) PlayerAttackLevels[attackType] = 0;
	}

	void Start () {
		towerPlacement = GetComponent<TowerPlacement>();
		gameManager = GameObject.FindGameObjectWithTag ("GameManager");

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("CycleAttack"))
		{
			curAttackTypeIndex = ((curAttackTypeIndex + 1) % attackTypeList.Length);
		}

		if ((Input.GetButton("Attack") || Input.GetMouseButton(1)) && !towerPlacement.IsTowerSelected()){
		    if (Input.GetButton("UnlockCage")) {
				GameObject target = gameManager.GetComponent<AStar>().target;
				if (target != null) {
					CageScript cage = target.GetComponent<CageScript>();
					if (cage != null) {
						if (!cage.isPlayerInRange()) {
							Attack(attackTypeList[curAttackTypeIndex]);
						}
					}
					else {
						Attack(attackTypeList[curAttackTypeIndex]);
					}
				}
			}
			else {
				Attack(attackTypeList[curAttackTypeIndex]);
			}
		}
	}

	private void Attack(AttackType attackType)
	{
		switch (attackType) {
			case AttackType.BasicBullet:
				BasicBulletAttack();
				break;
			case AttackType.SpreadBullet:
				SpreadBulletAttack();
				break;
			default:
				break;
		}
	}

	private void BasicBulletAttack()
	{
		if (Time.time > basicBulletTime + basicBulletDelay)
		{
			Vector3 velocity = (GetMousePos() - transform.position).normalized * basicBulletSpeed;
			
			Rigidbody2D attackInstance = Instantiate(basicBullet, transform.position, Quaternion.identity) as Rigidbody2D;
			attackInstance.GetComponent<BulletScript>().damage *= 1 + damageUpgradeMultiplier*PlayerAttackLevels[AttackType.BasicBullet];
			attackInstance.velocity = velocity;

			basicBulletTime = Time.time;
		}
	}

	private void SpreadBulletAttack()
	{
		if (Time.time > spreadBulletTime + spreadBulletDelay)
		{
			Vector3 velocityMain = (GetMousePos() - transform.position).normalized * spreadBulletSpeed;
			Vector3 velocitySide1 = RotateVector(velocityMain, 15).normalized * spreadBulletSpeed;
			Vector3 velocitySide2 = RotateVector(velocityMain, -15).normalized * spreadBulletSpeed;

			Rigidbody2D attackInstanceMain = Instantiate(spreadBullet, transform.position, Quaternion.identity) as Rigidbody2D;
			Rigidbody2D attackInstanceSide1 = Instantiate(spreadBullet, transform.position, Quaternion.identity) as Rigidbody2D;
			Rigidbody2D attackInstanceSide2 = Instantiate(spreadBullet, transform.position, Quaternion.identity) as Rigidbody2D;

			
			attackInstanceMain.GetComponent<BulletScript>().damage *= 1 + damageUpgradeMultiplier*PlayerAttackLevels[AttackType.SpreadBullet];
			attackInstanceSide1.GetComponent<BulletScript>().damage *= 1 + damageUpgradeMultiplier*PlayerAttackLevels[AttackType.SpreadBullet];
			attackInstanceSide2.GetComponent<BulletScript>().damage *= 1 + damageUpgradeMultiplier*PlayerAttackLevels[AttackType.SpreadBullet];

			attackInstanceMain.velocity = velocityMain;
			attackInstanceSide1.velocity = velocitySide1;
			attackInstanceSide2.velocity = velocitySide2;

			spreadBulletTime = Time.time;
		}
	}

	private Vector3 GetMousePos()
	{
		Vector3 worldLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		worldLocation.z = 0;

		return worldLocation;
	}

	private Vector2 RotateVector(Vector2 v, float degrees) {
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
		float tx = v.x;
		float ty = v.y;

		return new Vector2((cos * tx) - (sin * ty), (sin * tx) + (cos * ty));
	}
}
