using System;
using UnityEngine;
using System.Collections;

public class NaiveFollowPlayer : MonoBehaviour
{

    public GameObject GameManager;
    private GameManager _manager;
    public float CameraHeight = -10;

    private float yMin, yMax, xMin, xMax;
    private Camera camera;
    private int prevScreenWidth = -1, prevScreenHeight = -1;

	// Use this for initialization
	void Start ()
	{
	    _manager = GameManager.GetComponent<GameManager>();

	    camera = this.GetComponent<Camera>();
	}

    void ResetBorders()
    {
        var vertExt = camera.orthographicSize;
        var horzExt = vertExt * (float)Screen.width / (float)Screen.height;

        yMin = vertExt - 0.5f;
        xMin = horzExt - 0.5f;
        yMax = _manager.getBoardManager.MapHeight - vertExt - 0.5f;
        xMax = _manager.getBoardManager.MapWidth - horzExt - 0.5f;

        prevScreenWidth = Screen.width;
        prevScreenHeight = Screen.height;
    }
	
	// Update is called once per frame
    void Update()
    {
        if(Screen.width != prevScreenWidth || Screen.height != prevScreenHeight)
            ResetBorders();

	    this.transform.position = new Vector3(
            Math.Max(Math.Min(_manager.playerInstance.transform.position.x, xMax), xMin),
            Math.Max(Math.Min(_manager.playerInstance.transform.position.y, yMax), yMin),
            CameraHeight);
	}
}
