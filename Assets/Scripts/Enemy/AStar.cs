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

	private List<Node> nodeMesh;
	private int levelWidth;
	private LayerMask obstructionMask;
	public GameObject spawnPoint;
	private Node startNode;
	private Node targetNode;
	
	void Awake () {
		gameManager = GameObject.FindGameObjectWithTag("GameManager");
		boardManager = gameManager.GetComponent<BoardManager>();
		levelWidth = boardManager.MapWidth / boardManager.numLevels;

		obstructionMask = LayerMask.GetMask("Walls", "Towers");

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

	public void PlacedObstacleAt(int x, int y)
	{
		boardManager[x,y] = true;
		for (int i = 0; i < nodeMesh.Count; i++)
		{
			if (nodeMesh[i].x == x && nodeMesh[i].y == y)
				nodeMesh.RemoveAt(i);
		}

		AddToNodeMesh(x-1,y-1);
		AddToNodeMesh(x-1,y+1);
		AddToNodeMesh(x+1,y-1);
		AddToNodeMesh(x+1,y+1);

		RecalculateAdjNodeMesh();
		gameManager.GetComponent<WaveManagerScript>().RecalculatePaths();
		Debug.Log("nodeMesh recalculated for " + x + "," + y);
	}

	private void AddToNodeMesh(int x, int y)
	{
		if (boardManager[x,y])
			return;

		foreach (Node node in nodeMesh)
		{
			if (node.x == x && node.y == y)
				return;
		}
		nodeMesh.Add(new Node(x,y, true));
	}

	private void RecalculateAdjNodeMesh()
	{
		foreach (Node node in nodeMesh)
		{
			node.adjList = new List<Node>();
			node.adjWeightList = new List<float>();
		}
		GetAdjacencyList();
	}
		
		// Create a node for each neccessary tile on the map
	private void InitializeNodeMesh(int level)
	{
		nodeMesh = new List<Node>();
		int levelStartX = levelWidth * level;
		int levelEndX = levelStartX + levelWidth;

		for (int x = levelStartX; x < levelEndX; x++)
		{
			for (int y = 0; y < boardManager.MapHeight; y++)
			{
				if (isPartOfMesh(x, y))
				{
					nodeMesh.Add(new Node(x, y, !boardManager[x,y]));
				}
			}
		}
	}

	// Check if tile is a corner floor near a corner wall
	private bool isPartOfMesh(int x, int y)
	{
		if (!boardManager[x,y])
		{
			if ((x%5 == 0) && (y%5 == 0))
				return true;

			else if (boardManager[x-1,y-1])
				return !boardManager[x-1,y] && !boardManager[x,y-1];
			else if (boardManager[x-1,y+1])
				return !boardManager[x-1,y] && !boardManager[x,y+1];
			else if (boardManager[x+1,y+1])
				return !boardManager[x,y+1] && !boardManager[x+1,y];
			else if (boardManager[x+1,y-1])
				return !boardManager[x+1,y] && !boardManager[x,y-1];
		}
		return false;
	}

	// SHOULD DO THICKER RAY OR LINE CAST OR SOMETHINGA
	private void GetAdjacencyList()
	{
		RaycastHit2D[] zeroArray = new RaycastHit2D[1];
		int hit1, hit2;
		float thickness = 0.3f;
		Physics2D.raycastsStartInColliders = false;

		for (int nodeIndexA = 0; nodeIndexA < nodeMesh.Count; nodeIndexA++)
		{
			for (int nodeIndexB = nodeIndexA + 1; nodeIndexB < nodeMesh.Count; nodeIndexB++)
			{
				Node nodeA = nodeMesh[nodeIndexA];
				Node nodeB = nodeMesh[nodeIndexB];

				float distanceToTarget = (new Vector2(nodeB.x - nodeA.x, nodeB.y - nodeA.y)).magnitude;

				// lock to 20 x 20 square
				if (distanceToTarget > 8)
					continue;

				if ((nodeB.x < nodeA.x && nodeB.y > nodeA.y) || (nodeB.x > nodeA.x && nodeB.y < nodeA.y))
				{
					hit1 = Physics2D.LinecastNonAlloc(new Vector2(nodeA.x-thickness, nodeA.y-thickness), new Vector2(nodeB.x-thickness, nodeB.y-thickness), zeroArray, obstructionMask.value);
					hit2 = Physics2D.LinecastNonAlloc(new Vector2(nodeA.x+thickness, nodeA.y+thickness), new Vector2(nodeB.x+thickness, nodeB.y+thickness), zeroArray, obstructionMask.value);
				}
				else if ((nodeB.x < nodeA.x && nodeB.y < nodeA.y) || (nodeB.x > nodeA.x && nodeB.y > nodeA.y))
				{
					hit1 = Physics2D.LinecastNonAlloc(new Vector2(nodeA.x-thickness, nodeA.y+thickness), new Vector2(nodeB.x-thickness, nodeB.y+thickness), zeroArray, obstructionMask.value);
					hit2 = Physics2D.LinecastNonAlloc(new Vector2(nodeA.x+thickness, nodeA.y-thickness), new Vector2(nodeB.x+thickness, nodeB.y-thickness), zeroArray, obstructionMask.value);
				}
				else
				{
					hit1 = hit2 = Physics2D.LinecastNonAlloc(new Vector2(nodeA.x, nodeA.y), new Vector2(nodeB.x, nodeB.y), zeroArray, obstructionMask.value);
				}

				if (hit1 == 0 && hit2 == 0) {
					nodeA.adjList.Add(nodeB);
					nodeB.adjList.Add(nodeA);
					nodeA.adjWeightList.Add(distanceToTarget);
					nodeB.adjWeightList.Add(distanceToTarget);
				}
			}
		}
	}

	public void PrintNodeMesh(int level)
	{
		float t1,t2,t3,t4;
		t1 = Time.realtimeSinceStartup;
		Debug.Log("PrintNodeMesh " + t1);
		InitializeNodeMesh(level);
		t2 = Time.realtimeSinceStartup;
		Debug.Log("done: InitializeNodeMesh " + t2);
		GetAdjacencyList();
		t3 = Time.realtimeSinceStartup;
		Debug.Log("done: GetAdjacencyList " + t3);

		for (int nodeIndex = 0; nodeIndex < nodeMesh.Count; nodeIndex++)
		{
			Node node = nodeMesh[nodeIndex];
			Instantiate(spawnPoint, new Vector3(node.x, node.y, 0), Quaternion.identity);
			//Debug.Log("Node " + nodeIndex + " at " + node.x + "," + node.y);
			/*
			for (int adjIndex = 0; adjIndex < node.adjList.Count; adjIndex++)
			{
				Node adjNode = node.adjList[adjIndex];
				float adjNodeWeight = node.adjWeightList[adjIndex];
				//Debug.Log("   " + adjIndex + " " + adjNode.x + "," + adjNode.y + " " + adjNodeWeight);
			}
			*/
			//Debug.Log("");
		}
		t4 = Time.realtimeSinceStartup;
		Debug.Log("done: PrintingNodeMesh " + t4);
	}

	public void Initialize()
	{
		start = this.gameObject;
		if (target == null)
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
			toTarget = gameManager.GetComponent<GameManager>().PlayerInstance;
			Debug.Log("targetting player as " + toTarget + " at " + toTarget.transform.position);
		}
		if (toTarget == null)
		{
			// then no target found, so return self
			//Debug.Log("actually setting to start?");
			return start;
		}
		//Debug.Log("targetting cage as " + toTarget + " at " + toTarget.transform.position);
		return toTarget;
	}

	public void SetTarget(GameObject target)
	{
		this.target = target;
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
		public List<Node> adjList;
		public List<float> adjWeightList;

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
			adjList = new List<Node>();
			adjWeightList = new List<float>();

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

	public void ClearNodeMesh()
	{
		foreach(Node node in nodeMesh)
		{
			node.open = false;
			node.closed = false;
			node.parent = null;
		}
	}

	public void AddTargetToNodeMesh(GameObject target)
	{
		RaycastHit2D[] zeroArray = new RaycastHit2D[1];
		int hit1, hit2;
		Physics2D.raycastsStartInColliders = false;

		foreach(Node node in nodeMesh)
		{
			if (node.x == (int)target.transform.position.x && node.y == (int)target.transform.position.y)
			{
				targetNode = node;
				return;
			}
		}

		nodeMesh.Add(new Node((int)target.transform.position.x, (int)target.transform.position.y, true));
		for (int nodeIndexB = 0; nodeIndexB < nodeMesh.Count - 1; nodeIndexB++)
		{
			Node nodeA = nodeMesh[nodeMesh.Count - 1];
			Node nodeB = nodeMesh[nodeIndexB];
			
			if ((nodeB.x < nodeA.x && nodeB.y > nodeA.y) || (nodeB.x > nodeA.x && nodeB.y < nodeA.y))
			{
				hit1 = Physics2D.LinecastNonAlloc(new Vector2(nodeA.x-0.3f, nodeA.y-0.3f), new Vector2(nodeB.x-0.3f, nodeB.y-0.3f), zeroArray, obstructionMask.value);
				hit2 = Physics2D.LinecastNonAlloc(new Vector2(nodeA.x+0.3f, nodeA.y+0.3f), new Vector2(nodeB.x+0.3f, nodeB.y+0.3f), zeroArray, obstructionMask.value);
			}
			else if ((nodeB.x < nodeA.x && nodeB.y < nodeA.y) || (nodeB.x > nodeA.x && nodeB.y > nodeA.y))
			{
				hit1 = Physics2D.LinecastNonAlloc(new Vector2(nodeA.x-0.3f, nodeA.y+0.3f), new Vector2(nodeB.x-0.3f, nodeB.y+0.3f), zeroArray, obstructionMask.value);
				hit2 = Physics2D.LinecastNonAlloc(new Vector2(nodeA.x+0.3f, nodeA.y-0.3f), new Vector2(nodeB.x+0.3f, nodeB.y-0.3f), zeroArray, obstructionMask.value);
			}
			else
			{
				hit1 = hit2 = Physics2D.LinecastNonAlloc(new Vector2(nodeA.x, nodeA.y), new Vector2(nodeB.x, nodeB.y), zeroArray, obstructionMask);
			}
			
			if (hit1 == 0 && hit2 == 0) {
				float distanceToTarget = (new Vector2(nodeB.x - nodeA.x, nodeB.y - nodeA.y)).magnitude;
				
				nodeA.adjList.Add(nodeB);
				nodeB.adjList.Add(nodeA);
				nodeA.adjWeightList.Add(distanceToTarget);
				nodeB.adjWeightList.Add(distanceToTarget);
				//Debug.Log("target connect with " + nodeB.x + "," + nodeB.y + "hits " + hit1 + "," + hit2);
			}
			else
			{
				//Debug.Log("NOOO target connect with " + nodeB.x + "," + nodeB.y);
			}
			
		}
		targetNode = nodeMesh[nodeMesh.Count - 1];
		Debug.Log("adding target to nodeMesh at " + targetNode.x + "," + targetNode.y + " integ " + nodeMesh[nodeMesh.Count - 1].x + "," + nodeMesh[nodeMesh.Count - 1].y);
		//foreach(Node node in targetNode.adjList)
			//Debug.Log ("target adj is " + node.x + "," + node.y);
	}

	public void AddStartToNodeMesh(GameObject target)
	{
		RaycastHit2D[] zeroArray = new RaycastHit2D[1];
		RaycastHit2D[] zeroArray2 = new RaycastHit2D[1];
		int hit1, hit2;

		foreach(Node node in nodeMesh)
		{
			if (node.x == (int)target.transform.position.x && node.y == (int)target.transform.position.y)
			{
				startNode = node;
				return;
			}
		}
		Physics2D.raycastsStartInColliders = false;
		nodeMesh.Add(new Node((int)target.transform.position.x, (int)target.transform.position.y, true));
		Debug.Log("adding start to nodeMesh at " + nodeMesh[nodeMesh.Count - 1].x + "," + nodeMesh[nodeMesh.Count - 1].y);
		for (int nodeIndexB = 0; nodeIndexB < nodeMesh.Count - 1; nodeIndexB++)
		{
			Node nodeA = nodeMesh[nodeMesh.Count - 1];
			Node nodeB = nodeMesh[nodeIndexB];

			Vector2 toTarget = new Vector2(nodeB.x - nodeA.x, nodeB.y - nodeA.y);
			float distanceToTarget = toTarget.magnitude;
			
			if ((nodeB.x < nodeA.x && nodeB.y > nodeA.y) || (nodeB.x > nodeA.x && nodeB.y < nodeA.y))
			{
				hit1 = Physics2D.RaycastNonAlloc(new Vector2(nodeA.x-0.3f, nodeA.y-0.3f), toTarget.normalized, zeroArray, distanceToTarget, obstructionMask.value);
				hit2 = Physics2D.RaycastNonAlloc(new Vector2(nodeA.x+0.3f, nodeA.y+0.3f), toTarget.normalized, zeroArray2, distanceToTarget, obstructionMask.value);
			}
			else if ((nodeB.x < nodeA.x && nodeB.y < nodeA.y) || (nodeB.x > nodeA.x && nodeB.y > nodeA.y))
			{
				hit1 = Physics2D.RaycastNonAlloc(new Vector2(nodeA.x-0.3f, nodeA.y+0.3f), toTarget.normalized, zeroArray, distanceToTarget, obstructionMask.value);
				hit2 = Physics2D.RaycastNonAlloc(new Vector2(nodeA.x+0.3f, nodeA.y-0.3f), toTarget.normalized, zeroArray2, distanceToTarget, obstructionMask.value);
			}
			else
			{
				hit1 = hit2 = Physics2D.RaycastNonAlloc(new Vector2(nodeA.x, nodeA.y), toTarget.normalized, zeroArray, distanceToTarget, obstructionMask.value);
			}
			
			if (hit1 == 0 && hit2 == 0) {
				nodeA.adjList.Add(nodeB);
				nodeB.adjList.Add(nodeA);
				nodeA.adjWeightList.Add(distanceToTarget);
				nodeB.adjWeightList.Add(distanceToTarget);
				//Debug.Log("start connect with " + nodeB.x + "," + nodeB.y + "hits " + hit1 + "," + hit2 + " distance " + distanceToTarget + " direction " + toTarget + " mask " + obstructionMask.value);
			}
			else
			{
				//Debug.Log("NOOO start connect with " + nodeB.x + "," + nodeB.y);
				if (hit1 > 0)
				{
					//Debug.Log("1 connected with " + zeroArray[0].transform.position);
				}
				else
				{
					//Debug.Log("2 connected with " + zeroArray2[0].transform.position);
				}
			}
		}
		startNode = nodeMesh[nodeMesh.Count - 1];
	}

	void SetPathPoints2()
	{
		points = new List<Vector3>();
		// start at target
		Node node = targetNode;
		
		// then work backwards
		while(node.parent != null)
		{
			points.Add(new Vector3(node.x, node.y, 0));
			node = node.parent;
		}
		
		// Reverse the list so the target is last
		points.Reverse();
	}

	public void CalculateAStar2(GameObject start, GameObject target)
	{
		//Debug.Log("starting astar");

		AddStartToNodeMesh(start);
		AddTargetToNodeMesh(target);

		SortedList openList = new SortedList();
		
		// Initialize search
		Node start_n = startNode;
		start_n.g = 0;
		start_n.h = 10 * (int)(System.Math.Abs(targetNode.x - start_n.x) + System.Math.Abs(targetNode.y - start_n.y));
		start_n.f = start_n.g + start_n.h;
		start_n.closed = true;
		start_n.open = false;
		openList.Add((float)start_n.f + start_n.k, start_n);
		//openList.Add(start_n, start_n);
		openList.RemoveAt(0);
		//Debug.Log("starting node at " + start_n.x + "," + start_n.y);

		for (int adjNodeIndex = 0; adjNodeIndex < start_n.adjList.Count; adjNodeIndex++)
		{
			int g = (int)start_n.adjWeightList[adjNodeIndex];
			Node cur_n = start_n.adjList[adjNodeIndex];
			cur_n.parent = start_n;
			cur_n.g = g;
			cur_n.h = 10 * (int)(System.Math.Abs(targetNode.x - cur_n.x) + System.Math.Abs(targetNode.y - cur_n.y));
			cur_n.f = cur_n.g + cur_n.h;
			cur_n.open = true;
			openList.Add((double)cur_n.f + cur_n.k, cur_n);
			if (cur_n.x == targetNode.x && cur_n.y == targetNode.y)
			{
				Debug.Log("found target EARLY!");
				SetPathPoints2();
				return;
			}
			//Debug.Log("starting adj at " + cur_n.x + "," + cur_n.y);
		}

		//Debug.Log("initialized openlist");
		int cutoff = 0;
		// repeat search
		while(openList.Count > 0)
		{
			Node parent_n = openList.GetByIndex(0) as Node;
			openList.RemoveAt(0);

			//Debug.Log ("cutoff is " + cutoff++ + "with " + openList.Count + " checking " + parent_n.x + "," + parent_n.y);
			//if (cutoff > 2000)
			if (cutoff > 4000)
			{
				Debug.Log("GOT CUTOFF");
				SetPathPoints2();
				return;
			}

			for (int adjNodeIndex = 0; adjNodeIndex < parent_n.adjList.Count; adjNodeIndex++)
			{
				Node curNode = parent_n.adjList[adjNodeIndex];
				//Debug.Log("at adj at " + curNode.x + "," + curNode.y);
				if (!curNode.closed)
				{
					
					int g = (int)parent_n.adjWeightList[adjNodeIndex];
					
					// reached target
					if (curNode.x == targetNode.x && curNode.y == targetNode.y)
					{
						if (!curNode.open || curNode.g > parent_n.g + g)
						{
							Debug.Log("found target!");
							curNode.open = true;
							curNode.parent = parent_n;
							curNode.g = parent_n.g + g;
							curNode.f = curNode.g + curNode.h;
						}
						//return;
						//break;
					}

					else if (curNode.open)
					{
						if (curNode.g > parent_n.g + g)
						{	
							openList.Remove((double)curNode.f + curNode.k);
							
							curNode.parent = parent_n;
							curNode.g = parent_n.g + g;
							curNode.f = curNode.g + curNode.h;
							openList.Add((double)curNode.f + curNode.k, curNode);
							//openList.Add(nodeMap[x,y], nodeMap[x,y]);
						}
					}
					else
					{
						//Debug.Log("adding " + x + "," + y);
						
						curNode.parent = parent_n;
						curNode.g = parent_n.g + g;
						curNode.h = 10 * (int)(System.Math.Abs(targetNode.x - curNode.x) + System.Math.Abs(targetNode.y - curNode.y));
						curNode.f = curNode.g + curNode.h;
						curNode.open = true;
						openList.Add((double)curNode.f + curNode.k, curNode);
						//openList.Add(nodeMap[x,y], nodeMap[x,y]);
					}
				}
			}

			parent_n.closed = true;
			parent_n.open = false;
			//openList.RemoveAt(0);
			
		}
		Debug.Log("finished repeat loop");
		
		SetPathPoints2();
	}
}
