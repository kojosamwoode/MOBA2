using UnityEngine;
using System.Collections;
using System;

public class SetRotationTask : Task {

    Quaternion m_rotation;
    MobaEntity m_attacker;

    public SetRotationTask(MobaEntity attacker, Quaternion rotation, int time) : base(time)
    {
        m_attacker = attacker;
        m_rotation = rotation;
    }

    public override void Run()
    {
        m_attacker.transform.rotation = m_rotation;
        //Debug.Log("Face Target step" + NetworkTime.Instance.ServerStep());
    }

}
