using UnityEngine;
using System.Collections;

public class ExitGameButtonScript : MainMenuTextButtonScript {
	public override void onClick() {
		Application.Quit();
	}
}
