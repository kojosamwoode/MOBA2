using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Pathfinding : MonoSingleton<Pathfinding> {

	public Transform seeker, target;
	
	public Grid grid;

    [SerializeField]
    LayerMask mask1;
    [SerializeField]
    LayerMask mask2;
    Stopwatch sw;
    void Awake() {
        Physics.IgnoreLayerCollision(8, 12);
        Physics.IgnoreLayerCollision(12, 12);
        Physics.IgnoreLayerCollision(11, 12);
        CreateGrid(); 
    }

    public Node GeNearestNeighboursInRange(MobaEntity entity, Vector2 targetPos, float range)
    {
        //sw = new Stopwatch();
        //sw.Start();
        Node fromNode = GetNode(targetPos);       
        List<Node> neighbours = new List<Node>();
        Node nearestNeighbour = null;
        float curentNodeDistance = 100;
        for (int x = -5; x <= 5; x++)
        {
            for (int y = -5; y <= 5; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = fromNode.gridX + x;
                int checkY = fromNode.gridY + y;

                if (checkX >= 0 && checkX < grid.gridSizeX && checkY >= 0 && checkY < grid.gridSizeY)
                {
                    Node nodeToCheck = grid.grid[checkX, checkY];
                    if (nodeToCheck.walkable)
                    {
                        if (nodeToCheck.Resident)
                        {
                            if (nodeToCheck.Resident == entity)
                            {

                            }
                        }
                        else
                        {
                            neighbours.Add(nodeToCheck);
                        }
                    }
                }
            }
        }

        //Find the nearest node from the list
        foreach (Node currentNode in neighbours)
        {
            float distanceFromOrigin = Vector2.Distance(currentNode.m_position, entity.Position);
            float distanceToTarget = Vector2.Distance(currentNode.m_position, targetPos);
            if (distanceFromOrigin < curentNodeDistance && distanceToTarget <= range)
            {
                curentNodeDistance = distanceFromOrigin;
                nearestNeighbour = currentNode;
            }
        }
        //Let multiple minions overlap on the same node
        if (nearestNeighbour == null)
        {
            nearestNeighbour = neighbours[0];
        }
        nearestNeighbour.Resident = entity;
        return nearestNeighbour;
    }

    public Node GetNearestWalkableNode(Vector2 fromPos, Vector2 targetPos, float range)
    {
        Node fromNode = GetNode(targetPos);
        List<Node> neighbours = new List<Node>();
        Node nearestNeighbour = null;
        float curentNodeDistance = 100;
        for (int x = -5; x <= 5; x++)
        {
            for (int y = -5; y <= 5; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = fromNode.gridX + x;
                int checkY = fromNode.gridY + y;

                if (checkX >= 0 && checkX < grid.gridSizeX && checkY >= 0 && checkY < grid.gridSizeY)
                {
                    Node nodeToCheck = grid.grid[checkX, checkY];
                    if (nodeToCheck.walkable)
                    {
                        neighbours.Add(nodeToCheck);
                    }
                }
            }
        }

        //Find the nearest node from the list
        foreach (Node currentNode in neighbours)
        {
            float distanceFromOrigin = Vector2.Distance(currentNode.m_position, fromPos);
            float distanceToTarget = Vector2.Distance(currentNode.m_position, targetPos);
            if (distanceFromOrigin < curentNodeDistance && distanceToTarget <= range)
            {
                curentNodeDistance = distanceFromOrigin;
                nearestNeighbour = currentNode;
            }
        }
        if (nearestNeighbour != null)
        UnityEngine.Debug.Log("Nearest Walkable Node Finded");
        return nearestNeighbour;
    }
    public void CreateGrid()
    {
        grid = GetComponent<Grid>();
        grid.CreateGrid();
    }

    public Node CloseNode(Vector2 pos, MobaEntity entity)
    {
        Node node = grid.NodeFromWorldPoint(pos);
        node.walkable = false;
        node.Resident = entity;
        return node;
    }

    public Node GetNode(Vector2 pos)
    {
        return grid.NodeFromWorldPoint(pos);
    }

    public MobaPath FindPath(Vector2 startPos, Vector2 targetPos)
    {
        sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (!targetNode.walkable)
        {
            targetNode = GetNearestWalkableNode(startPos, targetPos, 1);
            if (targetNode == null)
            {
                UnityEngine.Debug.Log("Cannot find nearest walkable node");
                return null;
            }
        }

        if (startNode == targetNode)
        {
            if (Vector2.Distance(startPos, targetPos) > 0.1f)
            {
                List<Node> smoothPath = new List<Node>() { startNode, targetNode };
                Vector2[] pathCorners = new Vector2[] { startPos, targetNode.m_position };
                MobaPath path = new MobaPath(smoothPath, pathCorners);
                return path;
            }
            else
            {
                return null;
            }
           
        }

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                //sw.Stop();
                //UnityEngine.Debug.Log("Path calculated in: " + sw.ElapsedMilliseconds);
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                    {
                        //openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        return null;
    }

    List<Node> SmoothPath(List<Node> nodeList)
    {
        List<Node> newPath = new List<Node>();
        if (nodeList == null || nodeList.Count == 0)
        {
            return null;
        }
        Node startingNode = nodeList[0];
        newPath.Add(startingNode);
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (i == nodeList.Count - 1)
            {
                newPath.Add(nodeList[i]);
                continue;
            }
            GridPoints line = new GridPoints(new Vector2(startingNode.gridX, startingNode.gridY), new Vector2(nodeList[i].gridX, nodeList[i].gridY));
            foreach (Vector2 point in line)
            {
                Node currentNodepoint = Pathfinding.instance.grid.grid[(int)point.x, (int)point.y];
                if (!currentNodepoint.walkable)
                {
                    newPath.Add(nodeList[i - 1]);
                    startingNode = nodeList[i - 1];
                }
            }
        }
        return newPath;

    }


    MobaPath RetracePath(Node startNode, Node endNode) {
		List<Node> nodes = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			nodes.Add(currentNode);
			currentNode = currentNode.parent;
		}
		nodes.Reverse();

        if (nodes != null && nodes.Count>0)
        {
            List<Node> smoothPath = SmoothPath(nodes);
            Vector2[] pathCorners = new Vector2[smoothPath.Count];
            for (int i = 0; i < smoothPath.Count; i++)
            {
                pathCorners[i] = smoothPath[i].m_position;
            }
            MobaPath path = new MobaPath(smoothPath, pathCorners);
            sw.Stop();
            //UnityEngine.Debug.Log("Smooth Path calculated in: " + sw.ElapsedMilliseconds);
            return path;
        }
        return null;
	}

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}


}
