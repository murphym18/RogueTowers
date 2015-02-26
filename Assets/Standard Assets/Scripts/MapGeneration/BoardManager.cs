using UnityEngine;
using System.Collections.Generic;
using System;
using Helpers;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    private MapBuilder map;
    public int numLevels, levelWidth, levelHeight;

    public Transform boardHolder;

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
    }

    private void GenerateMap()
    {
        map = new MapBuilder(numLevels, levelWidth, levelHeight);
    }

    private void LayoutMap()
    {
        for (int y = 0; y < levelHeight; y++)
            for (int x = 0; x < numLevels * levelWidth; x++)
            {
                UnityEngine.Object instance;
                if (map[x, y])
                    instance = Instantiate(wallTiles.RandomChoice(), new Vector3(x, y), Quaternion.identity);
                else
                    instance = Instantiate(floorTiles.RandomChoice(), new Vector3(x, y), Quaternion.identity);
                ((GameObject)instance).transform.SetParent(boardHolder);
            }
    }

    public void SetupScene()
    {
        BoardSetup();
        GenerateMap();
        LayoutMap();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool this[int x, int y]
    {
        get { return map[x, y]; }
        set { map[x, y] = value; }
    }

    public int MapWidth
    {
        get { return numLevels * levelWidth; }
    }

    public int MapHeight
    {
        get { return levelHeight; }
    }
}
