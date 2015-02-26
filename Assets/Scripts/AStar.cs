using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AStar : MonoBehaviour {

	public bool useAStar = true;
	//public bool useAStar = false;

	private GameObject target;
	private GameObject start;
	private GameObject gameScript;
	private BoardManager boardScript;

	private List<Vector3> points = new List<Vector3>();
	private Node[,] nodeMap;

	// Use this for initialization
	void Awake () {
		start = this.gameObject;
		target = GetTarget();

		gameScript = GameObject.FindGameObjectWithTag("GameManager");
		boardScript = gameScript.GetComponent<BoardManager>();

		InitializeNodeMap();
		//CalculateAStar();

		//SetPathPoints();
		//SetBasicPathPoints();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GameObject GetTarget()
	{
		GameObject toTarget = GameObject.FindGameObjectWithTag ("Cage");
		if (toTarget == null)
		{
			// then cage was already destroyed, target player
			toTarget = GameObject.FindGameObjectWithTag ("Player");
		}
		if (toTarget == null)
		{
			// then no target found, so return self
			Debug.Log("actually setting to start?");
			return start;
		}

		return toTarget;
	}

	void SetPathPoints()
	{
		Node node = nodeMap[(int)target.transform.position.x,(int)target.transform.position.y];
		//Node node = nodeMap[(int)start.transform.position.x - 1,(int)start.transform.position.y - 1];
		while(node.parent != null)
		{
			Debug.Log("adding path: " + node.x + "," + node.y);

			points.Add(new Vector3(node.x, node.y, 0));
			node = node.parent;
		}
		points.Reverse();

	}

	void SetBasicPathPoints()
	{
		points.Clear();

		points.Add(start.transform.position + new Vector3(1, 1, 0));
		points.Add(start.transform.position + new Vector3(-1, 2, 0));
		points.Add(target.transform.position);
	}

	public List<Vector3> GetPoints()
	{
		if (useAStar)
		{
			//temp here
			CalculateAStar();
			//nodeMap[(int)target.transform.position.x,(int)target.transform.position.y].parent = nodeMap[(int)transform.position.x,(int)transform.position.y];
			SetPathPoints();
		}
		else
		{
			SetBasicPathPoints();
		}
		return points;
	}

	public class Node : IComparable<Node>
	{
		public int x, y;
		public bool walkable;
		public Node parent = null;
		public int f, g, h;
		public bool closed = false;
		public bool open = false;
		public double k;

		public Node(int x, int y, bool walkable)
		{
			this.x = x;
			this.y = y;
			this.walkable = walkable;
			this.k = (double)(x * 1000 + y)/1000000.0;
		}

		public int Compare (Node n1, Node n2)
		{
			return (n1.f < n2.f) ? 1 : -1;
		}

		public int CompareTo(Node n2)
		{
			return (f < n2.f) ? 1 : -1;
		}
	}
	/*
	public class NodeCompare<Node> : IComparer<Node>
	{
		int Compare(Node x, Node y)
		{
			return (((Node)x).f < ((Node)y).f) ? 1 : -1; 
		}
	}
	*/

	void InitializeNodeMap()
	{
		Debug.Log("initialzing nodemap");

		nodeMap = new Node[boardScript.MapWidth, boardScript.MapHeight];
		for (int x = 0; x < boardScript.MapWidth; x++)
		{
			for (int y = 0; y < boardScript.MapHeight; y++)
			{
				nodeMap[x,y] = new Node(x, y, !boardScript[x,y]);
			}
		}

		Debug.Log("nodemap initialized");
	}

	int ManhattanDistance(int x, int y)
	{
		return 10 * (int)(System.Math.Abs(target.transform.position.x - x) + System.Math.Abs(target.transform.position.y - y));
	}

	void CalculateAStar()
	{
		Debug.Log("starting astar");

		SortedList openList = new SortedList();

		// Initialize search
		Node start_n = nodeMap[(int)start.transform.position.x,(int)start.transform.position.y];
		start_n.g = 0;
		start_n.h = ManhattanDistance(start_n.x, start_n.y);
		start_n.f = start_n.g + start_n.h;
		start_n.closed = true;
		start_n.open = false;
		openList.Add((float)start_n.f + start_n.k, start_n);
		openList.RemoveAt(0);
		Debug.Log("placed starting node");
		//return;

		//for (int x = start_n.x - 1; x <= start_n.x + 1 && x < boardScript.MapWidth ; x++)
		for (int x = start_n.x - 1; x <= start_n.x + 1 && x < boardScript.MapWidth ; x++)
		{
			for (int y = start_n.y - 1; y <= start_n.y + 1; y++)
			{

				if (x >= 0 && y >= 0 && y < boardScript.MapHeight && !boardScript[x,y] && !nodeMap[x,y].closed)
				{
					Debug.Log("adding " + x + "," + y);
					int g = ((x - start_n.x) * (y - start_n.y)) == 0 ? 10 : 14;
					Node cur_n = nodeMap[x,y];
					cur_n.parent = nodeMap[(int)start.transform.position.x,(int)start.transform.position.y];
					cur_n.g = g;
					cur_n.h = ManhattanDistance(x,y);
					cur_n.f = cur_n.g + cur_n.h;
					cur_n.open = true;
					openList.Add((double)cur_n.f + cur_n.k, cur_n);
				}

			}
		}
		//return;
		//openList.Remove((double)start_n.f + start_n.k);

		// temp
		//Node mid = openList.GetByIndex(0) as Node;
		//nodeMap[(int)target.transform.position.x,(int)target.transform.position.y].parent = mid;
		//return;
		//

		Debug.Log("initialized openlist");

		// repeat search
		while(openList.Count > 0)
		{
			Node parent_n = openList.GetByIndex(0) as Node;
			// check surroundings
			for (int x = parent_n.x - 1; x <= parent_n.x + 1 && x < boardScript.MapWidth ; x++)
			{
				for (int y = parent_n.y - 1; y <= parent_n.y + 1; y++)
				{
					// if node exists and is valid
					if (x >= 0 && y >= 0 && y < boardScript.MapHeight && !boardScript[x,y] && !nodeMap[x,y].closed)
					{
						// reached target
						//if (x == (int)start.transform.position.x - 1 && y == (int)start.transform.position.y - 1)
						if (x == (int)target.transform.position.x && y == (int)target.transform.position.y)
						{
							Debug.Log("found target!");
							nodeMap[x,y].parent = parent_n;
							return;
							//break;
						}

						int g = ((x - parent_n.x) * (y - parent_n.y)) == 0 ? 10 : 14;
						if (nodeMap[x,y].open)
						{
							if (nodeMap[x,y].g > parent_n.g + g)
							{
								openList.Remove((double)nodeMap[x,y].f + nodeMap[x,y].k);
								nodeMap[x,y].parent = parent_n;
								nodeMap[x,y].g = parent_n.g + g;
								nodeMap[x,y].f = nodeMap[x,y].g + nodeMap[x,y].h;
								openList.Add((double)nodeMap[x,y].f + nodeMap[x,y].k, nodeMap[x,y]);
							}
						}
						else
						{
							Debug.Log("adding " + x + "," + y);

							nodeMap[x,y].parent = parent_n;
							nodeMap[x,y].g = parent_n.g + g;
							nodeMap[x,y].h = ManhattanDistance(x,y);
							nodeMap[x,y].f = nodeMap[x,y].g + nodeMap[x,y].h;
							nodeMap[x,y].open = true;
							openList.Add((double)nodeMap[x,y].f + nodeMap[x,y].k, nodeMap[x,y]);
						}
					}
				}
			}
			parent_n.closed = true;
			parent_n.open = false;
			openList.RemoveAt(0);

		}
		Debug.Log("finished repeat loop");
		// Form path

	}
}
