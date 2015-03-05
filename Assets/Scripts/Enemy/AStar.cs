using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AStar : MonoBehaviour {

	public bool useAStar = true;
	//public bool useAStar = false;

	private GameObject target;
	private GameObject start;
	private GameObject gameManager;
	private BoardManager boardManager;
	private SpawnPoint attachedSpawnPoint;

	private List<Vector3> points = new List<Vector3>();
	private Node[,] nodeMap;
	
	void Awake () {
		gameManager = GameObject.FindGameObjectWithTag("GameManager");
		boardManager = gameManager.GetComponent<BoardManager>();

		attachedSpawnPoint = GetComponent<SpawnPoint>();
		if (attachedSpawnPoint)
		{
			//Debug.Log("AStar for a spawnpoint");
		}
		else
		{
			//Debug.Log("AStar not for an enemy or not spawnpoint");
		}
	}

	public void Initialize()
	{
		start = this.gameObject;
		target = GetTarget();
		InitializeNodeMap();

		if (useAStar)
		{
			CalculateAStar();
			SetPathPoints();
		}
		else
		{
			SetBasicPathPoints();
		}
	}

	GameObject GetTarget()
	{
		GameObject toTarget = GameObject.FindGameObjectWithTag ("Cage");
		if (toTarget == null)
		{
			// then cage was already destroyed, target player
			toTarget = GameObject.FindGameObjectWithTag ("Player");
			Debug.Log(toTarget);
		}
		if (toTarget == null)
		{
			// then no target found, so return self
			//Debug.Log("actually setting to start?");
			return start;
		}

		return toTarget;
	}

	void SetPathPoints()
	{
		// start at target
		Node node = nodeMap[(int)target.transform.position.x,(int)target.transform.position.y];

		// then work backwards
		while(node.parent != null)
		{
			points.Add(new Vector3(node.x, node.y, 0));
			node = node.parent;
		}

		// Reverse the list so the target is last
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

	void InitializeNodeMap()
	{
		// Create a node for each square on the map
		nodeMap = new Node[boardManager.MapWidth, boardManager.MapHeight];
		for (int x = 0; x < boardManager.MapWidth; x++)
		{
			for (int y = 0; y < boardManager.MapHeight; y++)
			{
				nodeMap[x,y] = new Node(x, y, !boardManager[x,y]);
			}
		}
	}

	int ManhattanDistance(int x, int y)
	{
		return 10 * (int)(System.Math.Abs(target.transform.position.x - x) + System.Math.Abs(target.transform.position.y - y));
	}

	public class myReverserClass : IComparer  {
		
		// Calls CaseInsensitiveComparer.Compare with the parameters reversed. 
		int IComparer.Compare( System.Object x, System.Object y )  {

			return( (new Node(-1,-1,false)).Compare( (Node)x, (Node)y ) );
		}
		
	}

	bool notCornering(int start_x, int start_y, int x, int y)
	{
		if ((x - start_x) * (y - start_y) == 0)
			return true;
		else
		{
			int x_check = (start_x > x ? -1 : 1);
			int y_check = (start_y > y ? -1 : 1);
			return boardManager[start_x,y] || boardManager[x,start_y];
		}

		return true;
	}

	void CalculateAStar()
	{
		//Debug.Log("starting astar");

		SortedList openList = new SortedList();

		// Initialize search
		Node start_n = nodeMap[(int)start.transform.position.x,(int)start.transform.position.y];
		start_n.g = 0;
		start_n.h = ManhattanDistance(start_n.x, start_n.y);
		start_n.f = start_n.g + start_n.h;
		start_n.closed = true;
		start_n.open = false;
		openList.Add((float)start_n.f + start_n.k, start_n);
		//openList.Add(start_n, start_n);
		openList.RemoveAt(0);
		//Debug.Log("placed starting node");

		for (int x = start_n.x - 1; x <= start_n.x + 1 && x < boardManager.MapWidth ; x++)
		{
			for (int y = start_n.y - 1; y <= start_n.y + 1; y++)
			{

				if (x >= 0 && y >= 0 && y < boardManager.MapHeight && nodeMap[x,y].walkable && !nodeMap[x,y].closed)
				{
					//Debug.Log("adding " + x + "," + y);
					int g = ((x - start_n.x) * (y - start_n.y)) == 0 ? 10 : 14;
					Node cur_n = nodeMap[x,y];
					cur_n.parent = nodeMap[(int)start.transform.position.x,(int)start.transform.position.y];
					cur_n.g = g;
					cur_n.h = ManhattanDistance(x,y);
					cur_n.f = cur_n.g + cur_n.h;
					cur_n.open = true;
					openList.Add((double)cur_n.f + cur_n.k, cur_n);
					//openList.Add(start_n, start_n);
				}

			}
		}
		//Debug.Log("initialized openlist");

		// repeat search
		while(openList.Count > 0)
		{
			Node parent_n = openList.GetByIndex(0) as Node;
			openList.RemoveAt(0);
			// check surroundings
			for (int x = parent_n.x - 1; x <= parent_n.x + 1 && x < boardManager.MapWidth ; x++)
			{
				for (int y = parent_n.y - 1; y <= parent_n.y + 1; y++)
				{
					// if node exists and is valid
					if (x >= 0 && y >= 0 && y < boardManager.MapHeight && nodeMap[x,y].walkable && !nodeMap[x,y].closed &&
					    !(boardManager[parent_n.x,y] || boardManager[x,parent_n.y]))
					{
						// reached target
						if (x == (int)target.transform.position.x && y == (int)target.transform.position.y)
						{
							//Debug.Log("found target!");
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
								//openList.Add(nodeMap[x,y], nodeMap[x,y]);
							}
						}
						else
						{
							//Debug.Log("adding " + x + "," + y);

							nodeMap[x,y].parent = parent_n;
							nodeMap[x,y].g = parent_n.g + g;
							nodeMap[x,y].h = ManhattanDistance(x,y);
							nodeMap[x,y].f = nodeMap[x,y].g + nodeMap[x,y].h;
							nodeMap[x,y].open = true;
							openList.Add((double)nodeMap[x,y].f + nodeMap[x,y].k, nodeMap[x,y]);
							//openList.Add(nodeMap[x,y], nodeMap[x,y]);
						}
					}
				}
			}
			parent_n.closed = true;
			parent_n.open = false;
			//openList.RemoveAt(0);

		}
		Debug.Log("finished repeat loop");
	}
}
