using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public interface ICageEventTarget : IEventSystemHandler
{
	// functions that can be called via the messaging system
	void onCageUnlocked();
	void onCageShattered();
}
