using UnityEngine;
using System.Collections;

public class ChangeLevelButtonScript : MainMenuTextButtonScript {
	public string level = "GeneratedLevel";
    public string activationKey = "";
	public override void onClick() {
		Application.LoadLevel(level);
	}

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (activationKey != null && activationKey.Length > 0 && Input.GetButtonDown(activationKey))
            {
                onClick();
            }
        }
        base.Update();
    }
}
