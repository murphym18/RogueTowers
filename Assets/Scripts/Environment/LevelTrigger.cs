using UnityEngine;
using System.Collections;

public class LevelTrigger : MonoBehaviour
{

    public GameObject levelTransitionNotice;

    public int Level { get; set; }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && levelTransitionNotice != null)
        {
			GameObject.Find("GameManager").GetComponent<WaveManagerScript>().TriggerNextLevel(this.Level);
            levelTransitionNotice.SetActive(true);
            Destroy(levelTransitionNotice, 3.9f );
        }
    }
}
