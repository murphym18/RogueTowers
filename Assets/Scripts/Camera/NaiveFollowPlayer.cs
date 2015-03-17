using UnityEngine;
using System.Collections;

public class NaiveFollowPlayer : MonoBehaviour
{

    public GameObject GameManager;
    private GameManager _manager;
    public float CameraHeight = -10;

	// Use this for initialization
	void Start ()
	{
	    _manager = GameManager.GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
	    this.transform.position = new Vector3(
            _manager.playerInstance.transform.position.x,
            _manager.playerInstance.transform.position.y,
            CameraHeight);
	}
}
