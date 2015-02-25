using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

	public Collider2D[] enemySpawns;
	private int spawnInterval = 3;

	// Use this for initialization
	void Start () {
		// Get A* path to Cage

		// Spawn enemies
		StartCoroutine(SpawnAtInterval(spawnInterval));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator SpawnAtInterval(int seconds)
	{
		foreach (Collider2D enemyType in enemySpawns)
		{
			SpawnEnemy(enemyType);
			yield return new WaitForSeconds(seconds);
		}
	}

	void SpawnEnemy(Collider2D enemyType)
	{
		Instantiate (enemyType as BoxCollider2D, transform.position, Quaternion.identity);
	}


}
