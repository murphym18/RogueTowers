using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WobbleHeaderTextScript : MonoBehaviour {


    private Vector3 Base;

    private float LastUpdate;
    public float MoveDelay = 0.2f, Wobble = 0.7f;

	// Use this for initialization
	void Start(){
        Base = GetComponent<RectTransform>().position;
	}
	
	// Update is called once per frame


    void Update()
    {
        var curTime = Time.unscaledTime;
        if (curTime - LastUpdate > MoveDelay)
        {
            GetComponent<RectTransform>().position = new Vector3(Base.x + Random.Range(-Wobble, Wobble), Base.y + Random.Range(-Wobble, Wobble));
            LastUpdate = curTime;
        }
    }
}
