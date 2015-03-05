﻿using UnityEngine;
using System.Collections.Generic;

// The GameObject requires a RigidBody component

[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (BoxCollider2D))]
public class Tyrem : Character {
	
	public float maxSpeed = 5f;
	
	// Use this for initialization
	void Start () {

		
	}
	
	// Update is called once per frame
	void Update () {
		float h = Input.GetAxis ("MovePlayerHorizontal");
		float v = Input.GetAxis ("MovePlayerVertical");
		rigidbody2D.velocity = (new Vector2 (h, v)).normalized * maxSpeed;

		;
	}
}
