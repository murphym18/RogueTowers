using UnityEngine;
using System.Collections;

public class ChangeLevelButtonScript : MainMenuTextButtonScript {
	public string level = "GeneratedLevel";
	public override void onClick() {
		Application.LoadLevel(level);
	}
}
