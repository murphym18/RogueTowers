using UnityEngine;
using System.Collections;

public class IsometricObject : MonoBehaviour {
	
	// Update is called once per frame
	void LateUpdate ()
	{
	    this.GetComponent<SpriteRenderer>().sortingOrder = -(int) (this.transform.position.y*10);
	}
}
