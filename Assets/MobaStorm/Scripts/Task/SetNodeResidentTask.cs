using UnityEngine;
using System.Collections;
using System;

public class SetNodeResidentTask : Task {

    MobaEntity m_resident;
    Node m_node;

    public SetNodeResidentTask(MobaEntity resident, Vector2 position, int time) : base(time)
    {
        m_resident = resident;
        m_node = Pathfinding.instance.GetNode(position);
    }

    public override void Run()
    {
        m_node.Resident = m_resident;
    }

}
