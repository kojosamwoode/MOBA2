using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CalculatePathTask : Task {

    MobaEntity m_entity;
    MobaPath path;
    Vector2 m_startPos;

    public CalculatePathTask(MobaEntity entity, Vector2 startPos, Vector2 destinationPos, int time) : base(time)
    {
        m_startPos = startPos;
        m_entity = entity;
        if (Pathfinding.instance != null)
        {
            path = Pathfinding.instance.FindPath(startPos, destinationPos);
        }
    }

    public override void Run()
    {
        if (path == null)
        {
            return;
        }
        if (Vector2.Distance(m_entity.Position, m_startPos) > 0.2f)
        {
            Debug.Log("Correct Entity Position: ");
            m_entity.transform.position = Utils.Vector2ToVector3(m_startPos); 
        }
        //Debug.Log("Distance from original position: " + Vector2.Distance(m_entity.Position, m_startPos));
        m_entity.ClientSetNavMeshPath(path.Corners);
    }

}
