using UnityEngine;
using System.Collections.Generic;

public class Tyrem : Character {
	public enum Command {
		MOVE_NORTH,
		MOVE_NORTH_EAST,
		MOVE_EAST,
		MOVE_SOUTH_EAST,
		MOVE_SOUTH,
		MOVE_SOUTH_WEST,
		MOVE_WEST,
		MOVE_NORTH_WEST,
		PLACE_TOWER,
		ATTACK,
		PICK_LOCK
	};

	public Dictionary<Command, KeyCode[]> CommandKeys = new Dictionary<Command, KeyCode[]>();

	public float speed = 1f;

	// Use this for initialization
	void Start () {
		initDefaultControlls ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void initDefaultControlls() {
		CommandKeys [Command.MOVE_NORTH] = new KeyCode[]{KeyCode.W};
		CommandKeys [Command.MOVE_NORTH_EAST] = new KeyCode[]{KeyCode.W, KeyCode.D};
		CommandKeys [Command.MOVE_EAST] = new KeyCode[]{KeyCode.D};
		CommandKeys [Command.MOVE_SOUTH_EAST] = new KeyCode[]{KeyCode.S, KeyCode.D};
		CommandKeys [Command.MOVE_SOUTH] = new KeyCode[]{KeyCode.S};
		CommandKeys [Command.MOVE_SOUTH_WEST] = new KeyCode[]{KeyCode.S, KeyCode.A};
		CommandKeys [Command.MOVE_WEST] = new KeyCode[]{KeyCode.A};
		CommandKeys [Command.MOVE_NORTH_WEST] = new KeyCode[]{KeyCode.W, KeyCode.A};

		CommandKeys [Command.PLACE_TOWER] = new KeyCode[]{KeyCode.Space};
		CommandKeys [Command.ATTACK] = new KeyCode[]{KeyCode.LeftShift};
		CommandKeys [Command.PICK_LOCK] = new KeyCode[]{KeyCode.Tab};
	}
}
