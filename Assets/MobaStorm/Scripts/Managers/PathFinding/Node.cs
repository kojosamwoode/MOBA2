using UnityEngine;
using System.Collections;
[System.Serializable]
public class Node : IHeapItem<Node> {
	
	public bool walkable;
	public Vector2 m_position;
    public Vector3 WorldPosition
    {
        get { return new Vector3(m_position.x, 0, m_position.y); }
    }
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;

    private MobaEntity m_resident;
    public MobaEntity Resident
    {
        get { return m_resident; }
        set { m_resident = value; }
    }

    public float Distance(Vector2 pos)
    {
        return Vector2.Distance(pos, m_position);
    }

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
		walkable = _walkable;
        m_position = new Vector2(_worldPos.x, _worldPos.z);
		gridX = _gridX;
		gridY = _gridY;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
