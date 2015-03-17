using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public abstract class MainMenuTextButtonScript : MonoBehaviour {
	public Color mouseOverColor = Color.red;
	private RectTransform tx;
	private Text buttonText;
	private Color originalColor;
	// Use this for initialization
	void Start () {
		tx = gameObject.GetComponentInChildren<RectTransform>();
		buttonText = gameObject.GetComponentInChildren<Text>();
		originalColor = buttonText.color * Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		if (isMouseInButtonArea()) {
			buttonText.color = mouseOverColor;
			if (Input.GetMouseButtonDown(0)) {
				onClick();
			}
		}
		else {
			buttonText.color = originalColor;
		}

	}

	public abstract void onClick();

	private bool isMouseInButtonArea() {
		Vector3 pos = Input.mousePosition;
		Vector3[] corners = new Vector3[4]; 
		tx.GetWorldCorners(corners);
		float minX = corners[0].x;
		float maxX = corners[0].x;

		float minY = corners[0].y;
		float maxY = corners[0].y;
		foreach (Vector3 v in corners) {
			minX = Math.Min(minX, v.x);
			maxX = Math.Max(maxX, v.x);

			minY = Math.Min(minY, v.y);
			maxY = Math.Max(maxY, v.y);
		}

		minX -= 5;
		maxX += 5;
		minY -= 5;
		maxY += 5;

		return pos.x < maxX && pos.x > minX && pos.y < maxY && pos.y > minY;
	}
}
