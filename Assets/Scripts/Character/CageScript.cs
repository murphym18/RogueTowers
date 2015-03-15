using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CageScript : IsometricObject {

	public int hp;

	public float unlockDistance;
	public float unlockTime;

	public Color fullHealthBarColor = Color.green;
	public Color emptyHealthBarColor = Color.red;

	private float maxHp;
	private GameObject gameManager;
	private WaveManagerScript waveManager;
	private GameObject player;
	private float progress = 0F;
	private bool isUnlocked = false;

	private SpriteRenderer healthBarForegroundSprite;
	private SpriteRenderer healthBarBackgroundSprite;
	private GameObject lockPickClock;

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find("GameManager");
		player = GameObject.FindWithTag("Player");
		waveManager = gameManager.GetComponent<WaveManagerScript>();

		maxHp = (float) hp;
		healthBarForegroundSprite = this.transform.Find("CageHealthBarForeground").gameObject.GetComponent<SpriteRenderer>();
		healthBarBackgroundSprite = this.transform.Find("CageHealthBarBackground").gameObject.GetComponent<SpriteRenderer>();
		updateHealthBar();

	}
	
	// Update is called once per frame
	void Update () {
		updateCageLock();
	}

	private void updateCageLock() {
		if (!isUnlocked) {
			if (Input.GetButton ("UnlockCage")) {
				Vector3 d = this.transform.position - player.transform.position;
				
				if (d.sqrMagnitude < unlockDistance*unlockDistance) {
					progress += Time.deltaTime;
					if (unlockTime < progress) {
						isUnlocked = true;
						waveManager.TriggerCageUnlocked();
						
					}
					
					//ResetProgressMesh();
				}
			}
		}
	}

	public void damage() {
		if (!isUnlocked) {
			if (hp > 0) {
				hp -= 1;
				updateHealthBar();
				if (hp == 0) {
					waveManager.TriggerCageDestroyed();
					Destroy(gameObject);
				}
			}
		}
	}

	private void updateHealthBar() {
		float health = ((float)hp)/maxHp;
		Color healthBarColor = Color.Lerp(fullHealthBarColor, emptyHealthBarColor, 1 - health);
		healthBarForegroundSprite.material.color = healthBarColor;
		healthBarBackgroundSprite.material.color = healthBarColor;
		healthBarForegroundSprite.transform.localScale = new Vector3 (health, 1, 1);
	}

    /*

    public GameObject progressObjectType;
    private GameObject progressMeshObject;
    private Mesh prgMesh;
    public int ProgressMeshResolution = 360;

    void Awake()
    {
        progressMeshObject = Instantiate(progressObjectType, this.transform.position, Quaternion.identity) as GameObject;
        progressMeshObject.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        progressMeshObject.transform.parent = this.transform;
        InitializeProgressMesh();
        progressMeshObject.GetComponent<MeshFilter>().mesh = prgMesh;
    }

    void InitializeProgressMesh()
    {
        prgMesh = new Mesh();

        Vector3[] verts = new Vector3[ProgressMeshResolution + 1];
        verts[0] = new Vector3(0, 0);
        for(int i = 0; i < ProgressMeshResolution; i++)
            verts[i+1] = new Vector3(0, 1);
        prgMesh.vertices = verts;

        int[] triangles = new int[ProgressMeshResolution * 3];
        for (int i = 0; i < ProgressMeshResolution - 2; i++)
        {
            triangles[i*3] = 0;
            triangles[i*3 + 1] = i + 1;
            triangles[i*3 + 2] = i + 2;
        }
        triangles[ProgressMeshResolution*3 - 3] = 0;
        triangles[ProgressMeshResolution * 3 - 2] = ProgressMeshResolution;
        triangles[ProgressMeshResolution * 3 - 1] = 1;

        prgMesh.triangles = triangles;
    }

    void ResetProgressMesh()
    {
        var scaler = 2f*Math.PI/(float)ProgressMeshResolution * progress/unlockTime;
        for (int i = 1; i < ProgressMeshResolution + 1; i++)
        {
            var vec = new Vector3((float) Math.Cos((i - 1)*scaler), (float) Math.Sin((i - 1)*scaler));
            prgMesh.vertices[i] = vec;
            if (i == ProgressMeshResolution)
            {
                // Debug.Log("" + 2f * Math.PI + ", " + (float)ProgressMeshResolution + ", " + progress / unlockTime);
                // Debug.Log(scaler);
                // Debug.Log(vec);
            }
        }
    }
     * 
     */
}
