using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobaPath {
    
    public MobaPath(List<Node> nodes, Vector2[] corners)
    {
        m_nodes = nodes;
        m_corners = corners;
    }

    List<Node> m_nodes = new List<Node>();
    public List<Node> Nodes
    {
        get { return m_nodes; }
        set { m_nodes = value; }
    }

    private Vector2[] m_corners;
    public Vector2[] Corners
    {
        get { return m_corners; }
        set { m_corners = value; }
    }

}
