using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AStar : MonoBehaviour {

	private GameObject target;
	private Node startNode;
	private Node targetNode;
	private int oldTargetX, oldTargetY;
	private List<Node>[] nodeMesh;
	private List<Vector3> points = new List<Vector3>();
	private Queue<AStarJob> aStarQueue = new Queue<AStarJob>();

	private GameManager gameManager;
	private BoardManager boardManager;
	private WaveManagerScript waveManager;
	private LayerMask obstructionMask;

	void Awake () {
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		boardManager = gameManager.GetComponentInParent<BoardManager>();
		waveManager = gameManager.GetComponentInParent<WaveManagerScript>();
		nodeMesh = new List<Node>[boardManager.numLevels];

		obstructionMask = LayerMask.GetMask("Walls", "Towers", "Chests");
	}

	void Start()
	{
		//target = GetInitialTarget();
	}

	void Update()
	{
		if (TargetMoved())
		{
			BroadcastTargetMoved();
		}

		// does one astar calculation an update
		if (aStarQueue.Count > 0)
		{
			AStarJob curJob = aStarQueue.Dequeue();
			if (curJob.callback != null)
			{
				if (!CalculateAStar(curJob.start))
					points.Clear();
			    if (curJob.callback != null)
					curJob.callback.SendMessage("SetPathPointsCallback", points);
			}
		}
	}

	public void Initialize(int level)
	{
		target = GetInitialTarget(level);
		InitializeNodeMesh(level);
		FormAdjacencyList(level);
	}

	// Called by spawnpoints and enemies to get a calculated path
	public List<Vector3> GetPoints()
	{
		return points;
	}
	
	// Sets the AStar target. i.e. cage to player.
	public void SetTarget(GameObject newTarget)
	{
		this.target = newTarget;
		aStarQueue.Clear();
		gameManager.GetComponent<WaveManagerScript>().RecalculatePaths();
		BroadcastTargetMoved();
	}

	// Called by enemies and spawnpoints to queue up an astar
	//  path calculation from startingPoint to target. Sends a callback to caller.
	public void AStarEnqueue(Vector3 startingPoint, GameObject caller)
	{
		aStarQueue.Enqueue(new AStarJob(startingPoint, caller));
	}

	private class AStarJob
	{
		public Vector3 start;
		public GameObject callback;
		public AStarJob(Vector3 startingPosition, GameObject callback)
		{
			this.start = startingPosition;
			this.callback = callback;
		}
	}

	private void BroadcastTargetMoved()
	{
		aStarQueue.Clear();
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in enemies)
		{
			if (enemy != null)
				enemy.GetComponent<Enemy>().TellTargetMoved();
		}
	}

	// Called by TowerPlacement to recaclulate paths on tower down.
	public void PlacedObstacleAt(int x, int y)
	{
		boardManager[x,y] = true;
		for (int i = 0; i < nodeMesh[gameManager.currentLevel].Count; i++)
		{
			if (nodeMesh[gameManager.currentLevel][i].x == x && nodeMesh[gameManager.currentLevel][i].y == y)
				nodeMesh[gameManager.currentLevel].RemoveAt(i);
		}

		AddToNodeMesh(x-1,y-1);
		AddToNodeMesh(x-1,y+1);
		AddToNodeMesh(x+1,y-1);
		AddToNodeMesh(x+1,y+1);

		ReformAdjNodeMesh();
		gameManager.GetComponent<WaveManagerScript>().RecalculatePaths();
		BroadcastTargetMoved();
	}

	public bool TestObstacleAt(int x, int y)
	{
		bool removed = false, reached = false;
		boardManager[x,y] = true;
		for (int i = 0; i < nodeMesh[gameManager.currentLevel].Count; i++)
		{
			if (nodeMesh[gameManager.currentLevel][i].x == x && nodeMesh[gameManager.currentLevel][i].y == y)
			{
				nodeMesh[gameManager.currentLevel].RemoveAt(i);
				removed = true;
				break;
			}
		}
		
		AddToNodeMesh(x-1,y-1);
		AddToNodeMesh(x-1,y+1);
		AddToNodeMesh(x+1,y-1);
		AddToNodeMesh(x+1,y+1);
		
		ReformAdjNodeMesh();
		reached = waveManager.EnemiesCanReachTarget();
		boardManager[x,y] = false;
		if (removed)
			nodeMesh[gameManager.currentLevel].Add(new Node(x,y, true));
		return reached;
	}

	private void AddToNodeMesh(int x, int y)
	{
		if (boardManager[x,y])
			return;

		foreach (Node node in nodeMesh[gameManager.currentLevel])
		{
			if (node.x == x && node.y == y)
				return;
		}
		nodeMesh[gameManager.currentLevel].Add(new Node(x,y, true));
	}
	
	// Create a node for each neccessary tile on the map
	private void InitializeNodeMesh(int level)
	{
		nodeMesh[level] = new List<Node>();
		int levelStartX = level * (boardManager.MapWidth / boardManager.numLevels);
		int levelEndX = levelStartX + (boardManager.MapWidth / boardManager.numLevels);

		for (int c = 0; c < boardManager.numLevels; c++)
		{
			GameObject cage = boardManager.levelCages[c];
			int cage_x = (int)cage.transform.position.x;
			int cage_y = (int)cage.transform.position.y;
			for (int x = -1; x < 2; x++)
			{
				for (int y = -1; y < 2; y++)
				{
					boardManager[cage_x+x, cage_y+y] = true;
					nodeMesh[level].Add(new Node(cage_x+x, cage_y+y, false));
				}
			}
		}

		for (int x = levelStartX; x < levelEndX; x++)
		{
			for (int y = 0; y < boardManager.MapHeight; y++)
			{
				if (isPartOfMesh(x, y))
				{
					nodeMesh[level].Add(new Node(x, y, !boardManager[x,y]));
				}
			}
		}
	}

	// Check if tile is a corner floor near a corner wall
	//  or a multiple of 5x5.
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

	// For each node in waypoint mesh, fills its list of adjacent nodes
	// Limits to an 8 unit radius.
	private void FormAdjacencyList(int level)
	{
		for (int nodeIndexA = 0; nodeIndexA < nodeMesh[level].Count; nodeIndexA++)
		{
			for (int nodeIndexB = nodeIndexA + 1; nodeIndexB < nodeMesh[level].Count; nodeIndexB++)
			{
				Node nodeA = nodeMesh[level][nodeIndexA];
				Node nodeB = nodeMesh[level][nodeIndexB];

				// lock to 5 x 5 square
				float distanceToTarget = (new Vector2(nodeB.x - nodeA.x, nodeB.y - nodeA.y)).magnitude;
				if (distanceToTarget > 8)
					continue;

				if (NodesAreAdjacent(nodeA.x, nodeA.y, nodeB.x, nodeB.y))
				{
					nodeA.adjList.Add(nodeB);
					nodeB.adjList.Add(nodeA);
					nodeA.adjWeightList.Add(distanceToTarget);
					nodeB.adjWeightList.Add(distanceToTarget);
				}
			}
		}
	}
	
	// Could optimize
	public void ReformAdjNodeMesh()
	{
		foreach (Node node in nodeMesh[gameManager.currentLevel])
		{
			node.adjList = new List<Node>();
			node.adjWeightList = new List<float>();
		}
		FormAdjacencyList(gameManager.currentLevel);
	}

	// Checks if the target moved
	private bool TargetMoved()
	{
		if (target == null)
			return false;
		int newTargetX = (int)Mathf.Round(target.transform.position.x);
		int newTargetY = (int)Mathf.Round(target.transform.position.y);
		bool moved = oldTargetX != newTargetX ||
			oldTargetY != newTargetY;
		
		oldTargetX = newTargetX;
		oldTargetY = newTargetY;
		
		return moved;
	}

	// Gets a target if it exists. Cage -> Player -> Self.
	private GameObject GetInitialTarget(int level)
	{
		GameObject toTarget = boardManager.levelCages[level];
		if (toTarget == null)
			toTarget = gameManager.playerInstance;
		if (toTarget == null)
			return this.gameObject;
		
		oldTargetX = (int)Mathf.Round(toTarget.transform.position.x);
		oldTargetY = (int)Mathf.Round(toTarget.transform.position.y);
		
		return toTarget;
	}

	/* Code for actual AStar Calculation */

	// Node for AStar calculation
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

	public bool CalculateAStar(Vector3 start)
	{
		Vector3 toTarget = target.transform.position;
		return CalculateAStar(start, toTarget);
	}

	public bool CalculateAStar(Vector3 start, Vector3 target)
	{
		ClearNodeMesh();

		if (!(AddStartToNodeMesh(start) && AddTargetToNodeMesh(target)))
			return false;

		SortedList openList = new SortedList();
		
		// Initialize search
		Node start_n = startNode;
		start_n.g = 0;
		start_n.h = 10 * (int)(System.Math.Abs(targetNode.x - start_n.x) + System.Math.Abs(targetNode.y - start_n.y));
		start_n.f = start_n.g + start_n.h;
		start_n.closed = true;
		start_n.open = false;
		openList.Add((float)start_n.f + start_n.k, start_n);
		openList.RemoveAt(0);

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
				SetPathPoints();
				return true;
			}
		}
		
		// repeat search
		int cutoff = 4000;
		while(openList.Count > 0)
		{
			Node parent_n = openList.GetByIndex(0) as Node;
			openList.RemoveAt(0);

			if (cutoff-- < 0)
			{
				// Debug.Log("GOT CUTOFF");
				SetPathPoints();
				return true;
			}

			for (int adjNodeIndex = 0; adjNodeIndex < parent_n.adjList.Count; adjNodeIndex++)
			{
				Node curNode = parent_n.adjList[adjNodeIndex];
				if (!curNode.closed)
				{
					int g = (int)parent_n.adjWeightList[adjNodeIndex];
					
					// reached target
					if (curNode.x == targetNode.x && curNode.y == targetNode.y)
					{
						if (!curNode.open || curNode.g > parent_n.g + g)
						{
							//if (curNode.open)
								// Debug.Log("replacing best path with a " + curNode.g + " | " + (parent_n.g + g));
							curNode.open = true;
							curNode.parent = parent_n;
							curNode.g = parent_n.g + g;
							curNode.f = curNode.g + curNode.h;
							SetPathPoints();
							return true;
						}
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
						}
					}
					else
					{
						curNode.parent = parent_n;
						curNode.g = parent_n.g + g;
						curNode.h = 10 * (int)(System.Math.Abs(targetNode.x - curNode.x) + System.Math.Abs(targetNode.y - curNode.y));
						curNode.f = curNode.g + curNode.h;
						curNode.open = true;
						openList.Add((double)curNode.f + curNode.k, curNode);
					}
				}
			}
			parent_n.closed = true;
			parent_n.open = false;
			
		}
		return false;
	}

	// Sets points to calculated path
	// Stars at target node and works backwards through its parent
	void SetPathPoints()
	{
		points = new List<Vector3>();
		Node node = targetNode;

		while(node != null)
		{
			points.Add(new Vector3(node.x, node.y, 0));
			node = node.parent;
		}
		points.Reverse();
	}

	private bool AddStartToNodeMesh(Vector3 target)
	{
		Node nodeA = new Node((int)Mathf.Round(target.x), (int)Mathf.Round(target.y), true);
		startNode = GetNodeIfExists(nodeA.x, nodeA.y);
		if (startNode != null)
			return true;
		
		float closestDistance = float.PositiveInfinity;
		for (int nodeIndexB = 0; nodeIndexB < nodeMesh[gameManager.currentLevel].Count; nodeIndexB++)
		{
			Node nodeB = nodeMesh[gameManager.currentLevel][nodeIndexB];
			
			// lock to 10 x 10 square
			float distanceToTarget = (new Vector2(nodeB.x - nodeA.x, nodeB.y - nodeA.y)).magnitude;
			if (distanceToTarget > 8)
				continue;
			
			if (NodesAreAdjacentToPoint(nodeA.x, nodeA.y, nodeB.x, nodeB.y))
			{
				if (distanceToTarget < closestDistance)
				{
					startNode = nodeB;
					closestDistance = distanceToTarget;
				}
			}
		}
		return startNode != null;
	}
	
	private bool AddTargetToNodeMesh(Vector3 target)
	{
		Node nodeA = new Node((int)Mathf.Round(target.x), (int)Mathf.Round(target.y), true);
		targetNode = GetNodeIfExists(nodeA.x, nodeA.y);
		if (targetNode != null)
			return true;
		
		float closestDistance = float.PositiveInfinity;
		for (int nodeIndexB = 0; nodeIndexB < nodeMesh[gameManager.currentLevel].Count; nodeIndexB++)
		{
			Node nodeB = nodeMesh[gameManager.currentLevel][nodeIndexB];
			
			// lock to 10 x 10 square
			float distanceToTarget = (new Vector2(nodeB.x - nodeA.x, nodeB.y - nodeA.y)).magnitude;
			if (distanceToTarget > 8)
				continue;
			
			if (NodesAreAdjacentToPoint(nodeB.x, nodeB.y, nodeA.x, nodeA.y))
			{
				if (distanceToTarget < closestDistance)
				{
					targetNode = nodeB;
					closestDistance = distanceToTarget;
				}
			}
		}
		return targetNode != null;
	}
	
	private int ManhattanDistance(int x, int y)
	{
		return 10 * (int)(System.Math.Abs(target.transform.position.x - x) + System.Math.Abs(target.transform.position.y - y));
	}
	
	private bool notCornering(int start_x, int start_y, int x, int y)
	{
		if ((x - start_x) * (y - start_y) == 0)
			return true;
		else
		{
			return boardManager[start_x,y] || boardManager[x,start_y];
		}
	}
	
	private void ClearNodeMesh()
	{
		foreach(Node node in nodeMesh[gameManager.currentLevel])
		{
			node.open = false;
			node.closed = false;
			node.parent = null;
		}
	}
	
	// TODO: optimize
	private Node GetNodeIfExists(int x, int y)
	{
		foreach(Node node in nodeMesh[gameManager.currentLevel])
		{
			if (node.x == x && node.y == y)
			{
				return node;
			}
		}
		return null;
	}
	
	private bool NodesAreAdjacent(int startX, int startY, int endX, int endY)
	{
		RaycastHit2D[] zeroArray = new RaycastHit2D[1];
		int hit1, hit2;
		float thickness = 0.35f;//0.32f;
		Physics2D.raycastsStartInColliders = false;
		
		if ((endX < startX && endY > startY) || (endX > startX && endY < startY))
		{
			hit1 = Physics2D.LinecastNonAlloc(new Vector2(startX-thickness, startY-thickness), new Vector2(endX-thickness, endY-thickness), zeroArray, obstructionMask.value);
			hit2 = Physics2D.LinecastNonAlloc(new Vector2(startX+thickness, startY+thickness), new Vector2(endX+thickness, endY+thickness), zeroArray, obstructionMask.value);
		}
		else if ((endX < startX && endY < startY) || (endX > startX && endY > startY))
		{
			hit1 = Physics2D.LinecastNonAlloc(new Vector2(startX-thickness, startY+thickness), new Vector2(endX-thickness, endY+thickness), zeroArray, obstructionMask.value);
			hit2 = Physics2D.LinecastNonAlloc(new Vector2(startX+thickness, startY-thickness), new Vector2(endX+thickness, endY-thickness), zeroArray, obstructionMask.value);
		}
		else
		{
			hit1 = hit2 = Physics2D.LinecastNonAlloc(new Vector2(startX, startY), new Vector2(endX, endY), zeroArray, obstructionMask.value);
		}
		
		return (hit1 == 0 && hit2 == 0);
	}

	private bool NodesAreAdjacentToPoint(int startX, int startY, int endX, int endY)
	{
		RaycastHit2D[] zeroArray = new RaycastHit2D[1];
		int hit1, hit2;
		float thickness = 0.35f;//0.32f;
		Physics2D.raycastsStartInColliders = false;
		Vector2 end = new Vector2(endX, endY);
		
		if ((endX < startX && endY > startY) || (endX > startX && endY < startY))
		{
			hit1 = Physics2D.LinecastNonAlloc(new Vector2(startX-thickness, startY-thickness), end, zeroArray, obstructionMask.value);
			hit2 = Physics2D.LinecastNonAlloc(new Vector2(startX+thickness, startY+thickness), end, zeroArray, obstructionMask.value);
		}
		else if ((endX < startX && endY < startY) || (endX > startX && endY > startY))
		{
			hit1 = Physics2D.LinecastNonAlloc(new Vector2(startX-thickness, startY+thickness), end, zeroArray, obstructionMask.value);
			hit2 = Physics2D.LinecastNonAlloc(new Vector2(startX+thickness, startY-thickness), end, zeroArray, obstructionMask.value);
		}
		else
		{
			hit1 = hit2 = Physics2D.LinecastNonAlloc(new Vector2(startX, startY), end, zeroArray, obstructionMask.value);
		}

		return (hit1 == 0 && hit2 == 0);
	}

}
